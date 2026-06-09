using System.Diagnostics;
using System.Text;

namespace KStationPDF
{
    /// <summary>
    /// PDF オブジェクトストリーム圧縮解除ツールのメインフォーム。
    /// <para>
    /// qpdf を利用して、PDF 内のオブジェクトストリーム圧縮（/ObjStm）を解除します。
    /// Adobe Acrobat Pro の「最適化 → オブジェクト圧縮オプション → 圧縮を解除」と同等の処理を
    /// 無料で実行できるようにするためのツールです。
    /// </para>
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>qpdf.exe のパス。見つからない場合は null。</summary>
        private readonly string? qpdfPath;

        /// <summary>
        /// フォームを初期化し、qpdf の検出とイベント登録を行います。
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            qpdfPath = FindQpdf();

            // イベント登録
            panelDrop.DragEnter += PanelDrop_DragEnter;
            panelDrop.DragLeave += PanelDrop_DragLeave;
            panelDrop.DragDrop += PanelDrop_DragDrop;
            btnSelect.Click += BtnSelect_Click;
            btnClear.Click += (s, e) => txtLog.Clear();
            chkShowPassword.CheckedChanged += (s, e) =>
                txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

            // qpdf の検出結果に応じて初期状態を設定
            if (qpdfPath == null)
            {
                lblStatus.Text = "❌ qpdf.exe が見つかりません";
                lblStatus.ForeColor = Color.Red;
                AppendLog("❌ qpdf.exe が見つかりません。", Color.Red);
                AppendLog("   実行ファイルと同じフォルダ or qpdf/bin/ に配置してください。", Color.Orange);
                btnSelect.Enabled = false;
                panelDrop.AllowDrop = false;
            }
            else
            {
                AppendLog($"qpdf: {qpdfPath}", Color.Gray);
                AppendLog("PDFファイルをドロップまたは選択してください。\n", Color.White);
            }
        }

        #region qpdf 検索

        /// <summary>
        /// qpdf.exe を以下の優先順位で検索します。
        /// <list type="number">
        ///   <item>実行ファイルと同じフォルダ</item>
        ///   <item>サブフォルダ qpdf/bin/</item>
        ///   <item>システムの PATH 上</item>
        /// </list>
        /// </summary>
        /// <returns>qpdf.exe のパス。見つからない場合は null。</returns>
        private static string? FindQpdf()
        {
            var exeDir = AppContext.BaseDirectory;

            // 同じフォルダ
            var local = Path.Combine(exeDir, "qpdf.exe");
            if (File.Exists(local)) return local;

            // サブフォルダ qpdf/bin/
            var sub = Path.Combine(exeDir, "qpdf", "bin", "qpdf.exe");
            if (File.Exists(sub)) return sub;

            // PATH 上
            try
            {
                var psi = new ProcessStartInfo("qpdf", "--version")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using var p = Process.Start(psi);
                p?.WaitForExit(3000);
                if (p?.ExitCode == 0) return "qpdf";
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
        /// 指定された PDF ファイル群に対してオブジェクトストリーム圧縮の解除を実行します。
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
                    outputPath = Path.Combine(dir, $"{name}_圧縮解除.pdf");
                }

                // qpdf をバックグラウンドで実行
                (bool ok, string error) = await Task.Run(() => RunQpdf(file, outputPath, password));

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
                        AppendLog($"  ✅ 圧縮解除OK（/ObjStm なし）", Color.LimeGreen);
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

        #region qpdf 実行

        /// <summary>
        /// qpdf を実行してオブジェクトストリーム圧縮を解除します。
        /// パスワードが指定されている場合は、復号（--decrypt）も同時に行います。
        /// </summary>
        /// <param name="input">入力 PDF ファイルパス。</param>
        /// <param name="output">出力 PDF ファイルパス。</param>
        /// <param name="password">PDF のパスワード。不要な場合は null または空文字。</param>
        /// <returns>処理結果のタプル（成功フラグ, エラーメッセージ）。</returns>
        private (bool Success, string Error) RunQpdf(string input, string output, string? password)
        {
            try
            {
                var args = new List<string>();

                // パスワード付き PDF の復号
                if (!string.IsNullOrEmpty(password))
                {
                    args.Add($"--password=\"{password}\"");
                    args.Add("--decrypt");
                }

                // オブジェクトストリーム圧縮を解除
                args.Add("--object-streams=disable");

                args.Add($"\"{input}\"");
                args.Add($"\"{output}\"");

                var psi = new ProcessStartInfo
                {
                    FileName = qpdfPath!,
                    Arguments = string.Join(" ", args),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var process = Process.Start(psi)!;
                var stderr = process.StandardError.ReadToEnd();
                process.WaitForExit(30000);

                // 終了コード: 0=成功, 3=警告あり（処理自体は成功）
                if (process.ExitCode is 0 or 3)
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
        /// PDF バイナリ内を ASCII として読み込み、"/ObjStm" の文字列が含まれていないことを確認します。
        /// </summary>
        /// <param name="pdfPath">検証対象の PDF ファイルパス。</param>
        /// <returns>
        /// /ObjStm が存在しなければ true（圧縮解除成功）、
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