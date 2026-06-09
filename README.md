# KStationPDF

PDF ファイルのオブジェクトストリーム圧縮（/ObjStm）を解除する Windows GUI ツールです。

Adobe Acrobat Pro の「最適化 → オブジェクト圧縮オプション → 圧縮を解除」と
同等の処理を、無料で・誰でも・ドラッグ＆ドロップで実行できます。

## 💡 こんなときに

- PDF をアップロードするシステムが、オブジェクトストリーム圧縮に対応していない
- Acrobat Pro のライセンスがないスタッフでも処理を行いたい
- 大量の PDF を一括で処理したい

## ✨ 機能

- 📄 ドラッグ＆ドロップ / ファイル選択で PDF を処理
- 🔓 パスワード付き PDF の復号にも対応
- 📦 複数ファイルの一括処理
- ✅ 処理後の自動検証（/ObjStm の有無チェック）
- 🔄 上書きモード対応

## 📋 動作要件

- Windows 10 / 11
- [qpdf](https://github.com/qpdf/qpdf)（msvc64 版）

## 🚀 使い方

### ビルド済みバイナリを使う場合

1. [Releases](../../releases) からZIPをダウンロード
2. [qpdf](https://github.com/qpdf/qpdf/releases) の Windows 版（msvc64）をダウンロード
3. 解凍した `bin/` フォルダを `qpdf/bin/` としてアプリと同じ場所に配置
4. `KStationPDF.exe` を起動

### ソースからビルドする場合

```bash
git clone https://github.com/MetalOxid3/KStationPDF.git
cd KStationPDF
dotnet build -c Release
```

## 📂 フォルダ構成

```
📁 KStationPDF/
  ├── KStationPDF.exe
  └── qpdf/
       └── bin/
           ├── qpdf.exe
           └── ...
```

## 📜 License

MIT License. See [LICENSE](LICENSE) for details.

This software uses [qpdf](https://github.com/qpdf/qpdf) (Apache 2.0).
See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
