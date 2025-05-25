# UnityPackageマージツール

2つのUnityPackage（.unitypackage）をマージするシンプルなソフトウェアです。

## 使い方
- .NET 9.0でビルド済みの `bin\Debug\net9.0\MergeUnitypackage.exe` をそのまま使用可能です。
- 例: すべてのファイルを `C:\test` に置いた場合

```
C:\test>MergeUnitypackage.exe data/EmotePrefab-installer.unitypackage data/SampleEmote.unitypackage
Unity packages merged successfully!
```

- 第一引数・第二引数にunitypackageファイルを指定します。
- 第三引数で出力先ファイル名を指定できます（省略時は第一引数と同じフォルダに `Merged.unitypackage` を生成）。

## 機能
- 2つのunitypackageを一度解凍し、ファイル名・パスの衝突がなければ1つのフォルダにまとめて再圧縮します。
- uuid（GUID）やmetaファイルの内容までは厳密に検証しません。
- Unity Editorの完全なimport/export互換ではありませんが、一般的な用途には十分です。

## ビルド方法
- .NET 9.0 SDKが必要です。
- 依存ライブラリ: [SharpCompress](https://github.com/adamhathcock/sharpcompress)（MITライセンス）
- `dotnet build` でビルドできます。

## ライセンス
- 本ソフトウェアはMITライセンスです。
- 依存ライブラリSharpCompressもMITライセンスです。

## 作成意図
- [VPAI](https://github.com/anatawa12/VPAI-unitypackages)を他のunitypackageに組み込む用途などを想定しています。
