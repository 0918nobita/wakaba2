# OUJ CLI

放送大学の VOD システムを快適に利用するための CLI ツール

## 準備

`.env` ファイルを作成し、

- Google Chrome の実行ファイルのパス
- 学生アカウントのユーザーネーム
- 学生アカウントのパスワード

を書き込んで保存する

```text
CHROME_EXE_PATH=...
OUJ_USERNAME=...
OUJ_PASSWORD=...
```

## ビルド方法

```bash
npm run build
```

## 起動方法

```bash
./bin/run
```

## 検討中の機能

- ログインセッションを保持する
- 講義動画のブックマーク
- ブックマークされた講義動画のリスト表示
  - 項目を選択して対応する再生ページに遷移する
