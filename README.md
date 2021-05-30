# Wakaba 2

放送大学の Web アプリを快適に利用するための Chrome 拡張機能

## 事前インストールが必要なもの

- Node.js v14
- .NET 5.0 SDK

## インストール手順

1. 依存している npm パッケージをインストールする

```bash
npm ci
```

2. 依存している nuget パッケージをインストールする

```bash
dotnet tool restore
dotnet paket restore
dotnet restore
```

3. 拡張機能をビルドする

```bash
npm run build # または npm run build:prod
```

4. ``/dist`` をパッケージ化していない拡張機能として Chrome に読み込む

## VS Code DevContainer を利用して開発する

VS Code でこのリポジトリのルートディレクトリを開き、コマンドパレットから ``Remote-Containers: Reopen in Container`` コマンドを実行すると Node.js + .NET 5 の開発環境が整います

## Linux 版の Chrome で利用する場合の注意点

UserAgent が Linux 版の Chrome のものになっていると VOD システムが利用できないため、  
以下のようにスクリプトから Chrome を起動するようにしてください。  
(一時的に UserAgent が Windows 版 Chrome のものに上書きされます)

```bash
./scripts/launch
```
