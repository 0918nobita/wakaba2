# Wakaba 2

放送大学の Web アプリを快適に利用するための Chrome 拡張機能

## インストール方法

``/dist`` をパッケージ化していない拡張機能として Chrome に読み込む

## 起動手順

1. Chrome を ``--remote-debugging-port`` オプション付きで起動する

```bash
./scripts/launch-chrome
```

2. ↑は実行中のままで別途シェルを立ち上げて、Chrome の UserAgent 設定を一時的に上書きするための Node.js プログラムを起動する

```bash
./scripts/setup
```
