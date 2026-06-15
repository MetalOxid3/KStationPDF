# KStationPDF v2.0.0

PDF ファイルのオブジェクトストリーム圧縮（/ObjStm）を解除し、画像の再エンコードを行うことで、サーバーへのアップロード互換性を確保する Windows デスクトップアプリケーションです。

Adobe Acrobat Pro の「最適化」と同等の処理を無料で実行できます。

## ✨ 特徴

- **Ghostscript による PDF 再構築** — /ObjStm 解除、画像再エンコード、PDF 1.4 互換化を一括処理
- **ドラッグ＆ドロップ対応** — PDF ファイルをドロップするだけで処理開始
- **複数ファイル一括処理** — フォルダ内の PDF をまとめて処理可能
- **パスワード付き PDF 対応** — 暗号化された PDF の復号も同時に実行
- **処理結果の自動検証** — /ObjStm が残っていないことを自動チェック
- **上書きモード** — 元ファイルを直接置換するオプション付き
- **Ghostscript 自動検出** — インストール済みの Ghostscript を自動で検出

## 📋 必要な環境

| 項目 | 要件 |
|---|---|
| OS | Windows 10 / 11（64bit） |
| ランタイム | .NET 8.0 Desktop Runtime |
| **Ghostscript** | **別途インストールが必要**（後述） |

### Ghostscript のインストール

本ツールの動作には **Ghostscript** が必要です。以下の手順でインストールしてください。

1. [Ghostscript 公式サイト](https://ghostscript.com/releases/gsdnld.html) を開く
2. **Ghostscript x.xx.x for Windows (64 bit)** の **Ghostscript AGPL Release** をダウンロード
3. インストーラを実行（デフォルト設定で OK）

> ⚠️ Ghostscript は AGPL ライセンスのため、本ツールには同梱していません。  
> ユーザーご自身でインストールをお願いします。

## 🚀 使い方

### 1. ダウンロード

[Releases](../../releases) ページから最新版の zip をダウンロードし、任意のフォルダに展開します。

### 2. 起動

`KStationPDF.exe` を実行します。  
Ghostscript が正しくインストールされていれば、自動検出されます。

### 3. PDF を処理

- **ドラッグ＆ドロップ**: PDF ファイルをアプリのドロップエリアにドラッグ
- **ファイル選択**: 「ファイル選択」ボタンから PDF を選択

処理が完了すると `元ファイル名_処理済み.pdf` が同じフォルダに生成されます。

### 4. オプション

| オプション | 説明 |
|---|---|
| 上書きモード | 元ファイルを処理済みファイルで置換 |
| パスワード入力 | 暗号化 PDF の復号パスワードを指定 |

## 🔧 ソースからビルド

```bash
git clone https://github.com/MetalOxid3/KStationPDF.git
cd KStationPDF
dotnet build -c Release
```

### 単一ファイル発行

```bash
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

## 📁 ファイル構成

```
KStationPDF/
├── KStationPDF.exe      # アプリケーション本体
├── README.md
├── LICENSE
└── THIRD-PARTY-NOTICES.md
```

> Ghostscript は別途インストールされている前提です（同梱していません）。

## 🔄 v1 からの移行について

### v1（qpdf ベース）
- qpdf を同梱し、PDF の構造整理（/ObjStm 解除）のみを実行
- 一部のスキャン PDF（CCITTFaxDecode 等の画像を含む場合）でサーバーエラーが発生する場合がありました

### v2（Ghostscript ベース）⬅ 現在のバージョン
- Ghostscript による PDF の完全再構築を実行
- 画像の再エンコード（CCITT → JPEG 等）も行うため、より広い互換性を実現
- **Ghostscript の別途インストールが必要** になりました

## 📄 ライセンス

本ツールは [MIT License](LICENSE) で公開しています。

### Ghostscript について

本ツールは外部ツールとして [Ghostscript](https://ghostscript.com/) を利用します。  
Ghostscript は [GNU AGPL v3](https://www.gnu.org/licenses/agpl-3.0.html) ライセンスで提供されており、本ツールには同梱していません。  
詳細は [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) を参照してください。
