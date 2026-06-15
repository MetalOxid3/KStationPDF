using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KStationPDF
{
    /// <summary>
    /// PDF 圧縮解除ツール v2 メインフォーム。
    /// <para>
    /// Ghostscript を利用して PDF を再構築し、オブジェクトストリーム圧縮（/ObjStm）の解除、
    /// 画像の再エンコード、および PDF 1.4 互換化を行います。
    /// Adobe Acrobat Pro の「最適化」と同等の処理を無料で実行できます。
    /// </para>
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>gswin64c.exe のパス。見つからない場合は null。</summary>
        private readonly string? gsPath;

        /// <summary>
        /// フォームを初期化し、Ghostscript の検出とイベント登録を行います。
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            gsPath = FindGhostscript();

            // イベント登録
            panelDrop.DragEnter += PanelDrop_DragEnter;
            panelDrop.DragLeave += PanelDrop_DragLeave;
            panelDrop.DragDrop += PanelDrop_DragDrop;
            btnSelect.Click += BtnSelect_Click;
            btnClear.Click += (s, e) => txtLog.Clear();
            chkShowPassword.CheckedChanged += (s, e) =>
                txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

            // Ghostscript の検出結果に応じて初期状態を設定
            if (gsPath == null)
            {
                lblStatus.Text = "❌ Ghostscript が見つかりません";
                lblStatus.ForeColor = Color.Red;
                AppendLog("❌ Ghostscript（gswin64c.exe）が見つかりません。", Color.Red);
                AppendLog("   https://ghostscript.com からインストールしてください。", Color.Orange);
                btnSelect.Enabled = false;
                panelDrop.AllowDrop = false;
            }
            else
            {
                AppendLog($"Ghostscript: {gsPath}", Color.Gray);
                AppendLog("PDFファイルをドロップまたは選択してください。\n", Color.White);
            }
        }

        #region Ghostscript 検索

        /// <summary>
        /// gswin64c.exe を以下の優先順位で検索します。
        /// <list type="number">
        ///   <item>Program Files 配下の gs フォルダ（最新バージョン優先）</item>
        ///   <item>Program Files (x86) 配下の gs フォルダ</item>
        ///   <item>システムの PATH 上</item>
        /// </list>
        /// </summary>
        /// <returns>gswin64c.exe のパス。見つからない場合は null。</returns>
        private static string? FindGhostscript()
        {
            // Program Files 配下を探索
            string[] searchRoots =
            [
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            ];

            foreach (var root in searchRoots)
            {
                var gsRoot = Path.Combine(root, "gs");
                if (!Directory.Exists(gsRoot)) continue;

                // バージョンフォルダを降順ソート（最新優先）
                var versionDirs = Directory.GetDirectories(gsRoot)
                    .OrderByDescending(d => d)
                    .ToArray();

                foreach (var dir in versionDirs)
                {
                    var candidate = Path.Combine(dir, "bin", "gswin64c.exe");
                    if (File.Exists(candidate)) return candidate;

                    // 32bit版フォールバック
                    candidate = Path.Combine(dir, "bin", "gswin32c.exe");
                    if (File.Exists(candidate)) return candidate;
                }
            }

            // PATH 上を検索
            try
            {
                var psi = new ProcessStartInfo("gswin64c", "--version")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using var p = Process.Start(psi);
                p?.WaitForExit(3000);
                if (p?.ExitCode == 0) return "gswin64c";
            }
            catch { }

            return null;
        }

        #endregion

        #region ドラッグ＆ドロップ

        /// <summary>ドロップ領域にファイルが入ったときの視覚フィードバック。</summary>
        private void PanelDrop_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
                panelDrop.BackColor = Color.FromArgb(200, 225, 255);
                lblDrop.Text = "📥 ドロップして処理開始！";
            }
        }

        /// <summary>ドロップ領域からファイルが離れたときに表示を元に戻す。</summary>
        private void PanelDrop_DragLeave(object? sender, EventArgs e)
        {
            panelDrop.BackColor = Color.FromArgb(240, 245, 255);
            lblDrop.Text = "📄 ここに PDF ファイルをドラッグ＆ドロップ";
        }

        /// <summary>ファイルがドロップされたとき、PDF のみを抽出して処理を開始する。</summary>
        private void PanelDrop_DragDrop(object? sender, DragEventArgs e)
        {
            panelDrop.BackColor = Color.FromArgb(240, 245, 255);
            lblDrop.Text = "📄 ここに PDF ファイルをドラッグ＆ドロップ";

            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files)
            {
                var pdfFiles = files
                    .Where(f => f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (pdfFiles.Length > 0)
                    _ = ProcessFilesAsync(pdfFiles);
                else
                    AppendLog("⚠️ PDFファイルが含まれていません。\n", Color.Yellow);
            }
        }

        #endregion

        #region ファイル選択ダイアログ

        /// <summary>ファイル選択ダイアログを表示し、選択された PDF の処理を開始する。</summary>
        private void BtnSelect_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "PDFファイルを選択",
                Filter = "PDF ファイル (*.pdf)|*.pdf",
                Multiselect = true,
            };

            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileNames.Length > 0)
                _ = ProcessFilesAsync(ofd.FileNames);
        }

        #endregion

        #region メイン処理

        /// <summary>
        /// 指定された PDF ファイル群に対して Ghostscript による再構築処理を実行します。
        /// パスワードが入力されている場合は、復号も同時に行います。
        /// </summary>
        /// <param name="files">処理対象の PDF ファイルパス配列。</param>
        private async Task ProcessFilesAsync(string[] files)
        {
            // UI をロック
            btnSelect.Enabled = false;
            panelDrop.AllowDrop = false;
            progressBar1.Value = 0;
            progressBar1.Maximum = files.Length;

            // パスワードを UI スレッドで取得しておく
            string password = txtPassword.Text;

            int success = 0, fail = 0;

            AppendLog($"━━━ 処理開始: {files.Length} ファイル ━━━", Color.Cyan);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                AppendLog($"\n▶ {fileName}", Color.White);

                // 出力先パスを決定
                string outputPath;
                if (chkOverwrite.Checked)
                {
                    // 上書きモード: 一時ファイルに出力 → 成功後に置換
                    outputPath = file + ".tmp";
                }
                else
                {
                    var dir = Path.GetDirectoryName(file)!;
                    var name = Path.GetFileNameWithoutExtension(file);
                    outputPath = Path.Combine(dir, $"{name}_処理済み.pdf");
                }

                // Ghostscript をバックグラウンドで実行
                (bool ok, string error) = await Task.Run(() => RunGhostscript(file, outputPath, password));

                if (ok)
                {
                    string verifyPath = chkOverwrite.Checked ? file : outputPath;

                    // 上書きモードの場合、一時ファイルで元ファイルを置換
                    if (chkOverwrite.Checked)
                    {
                        try
                        {
                            File.Delete(file);
                            File.Move(outputPath, file);
                        }
                        catch (Exception ex)
                        {
                            AppendLog($"  ❌ 上書き失敗: {ex.Message}", Color.Red);
                            fail++;
                            progressBar1.Value++;
                            continue;
                        }
                    }

                    // 処理結果の検証
                    if (VerifyNoObjectStreams(verifyPath))
                    {
                        AppendLog($"  ✅ 処理OK（/ObjStm なし）", Color.LimeGreen);
                        success++;
                    }
                    else
                    {
                        AppendLog($"  ⚠️ 処理完了したが /ObjStm が残っています", Color.Orange);
                        success++;
                    }
                }
                else
                {
                    AppendLog($"  ❌ エラー: {error}", Color.Red);

                    // 上書きモードで失敗した場合、一時ファイルを削除
                    if (chkOverwrite.Checked && File.Exists(outputPath))
                        try { File.Delete(outputPath); } catch { }

                    fail++;
                }

                progressBar1.Value++;
            }

            // 完了表示
            AppendLog($"\n━━━ 完了: 成功 {success} / 失敗 {fail} ━━━\n", Color.Cyan);
            lblStatus.Text = $"✅ 処理完了 — 成功: {success}, 失敗: {fail}";

            // UI のロックを解除
            btnSelect.Enabled = true;
            panelDrop.AllowDrop = true;
        }

        #endregion

        #region Ghostscript 実行

        /// <summary>
        /// Ghostscript を実行して PDF を再構築します。
        /// PDF 1.4 互換で出力し、オブジェクトストリームの解除と画像の再エンコードを行います。
        /// パスワードが指定されている場合は、復号も同時に行います。
        /// </summary>
        /// <param name="input">入力 PDF ファイルパス。</param>
        /// <param name="output">出力 PDF ファイルパス。</param>
        /// <param name="password">PDF のパスワード。不要な場合は null または空文字。</param>
        /// <returns>処理結果のタプル（成功フラグ, エラーメッセージ）。</returns>
        private (bool Success, string Error) RunGhostscript(string input, string output, string? password)
        {
            try
            {
                var args = new List<string>
                {
                    "-sDEVICE=pdfwrite",
                    "-dCompatibilityLevel=1.4",
                    "-dNOPAUSE",
                    "-dBATCH",
                    "-dQUIET",
                };

                // パスワード付き PDF の復号
                if (!string.IsNullOrEmpty(password))
                {
                    args.Add($"-sPDFPassword={password}");
                }

                args.Add($"-sOutputFile={output}");
                args.Add(input);

                var psi = new ProcessStartInfo
                {
                    FileName = gsPath!,
                    Arguments = string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var process = Process.Start(psi)!;
                var stderr = process.StandardError.ReadToEnd();
                process.WaitForExit(120000); // Ghostscript は重いので 2分

                if (process.ExitCode == 0)
                    return (true, string.Empty);

                return (false, string.IsNullOrWhiteSpace(stderr)
                    ? $"終了コード: {process.ExitCode}"
                    : stderr.Trim());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        #endregion

        #region ユーティリティ

        /// <summary>
        /// ログ領域に色付きテキストを追記します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="color">テキストの色。</param>
        private void AppendLog(string text, Color color)
        {
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionColor = color;
            txtLog.AppendText(text + "\n");
            txtLog.ScrollToCaret();
        }

        /// <summary>
        /// 処理済み PDF にオブジェクトストリーム（/ObjStm）が残っていないことを検証します。
        /// </summary>
        /// <param name="pdfPath">検証対象の PDF ファイルパス。</param>
        /// <returns>
        /// /ObjStm が存在しなければ true（処理成功）、
        /// 存在する場合やファイルが読めない場合は false。
        /// </returns>
        private static bool VerifyNoObjectStreams(string pdfPath)
        {
            try
            {
                var bytes = File.ReadAllBytes(pdfPath);
                var text = Encoding.ASCII.GetString(bytes);
                return !text.Contains("/ObjStm");
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}