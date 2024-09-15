- 2つのUnityPackageをマージするソフトウェア

- 使い方
  - コンパイルする(.NET Framework 4.7.2で作成)
  - 生成されたファイルを仮にすべてC:\test以下に置いた場合、次の通り

 ```
C:\test>MergeUnitypackage.exe data/EmotePrefab-installer.unitypackage data/SampleEmote.unitypackage
Unity packages merged successfully!
```

- 機能
  - 第一変数と第二変数にunitypackageを指定して起動する。一応第三変数に出力先指定も可能（省略時、第一と同じフォルダに生成）
  - 次の動作を実行する
    - 入力された2つのunitypackage(Input1,Input2)を.tar.gzとして解凍する
    - 普通のファイルとして１つのフォルダに入れる
    - フォルダを.tar.gzとして圧縮し、拡張子をunitypackageに変更する
   
- 作成意図
  - [VPAI](https://github.com/anatawa12/VPAI-unitypackages)を他のunitypackageに組み込む為
