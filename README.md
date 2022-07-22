# これはなに？
Zenject(Extenject)を用いたステートマシン

# どうやって使う？

## 状態の定義
```
// 状態の識別子を列挙体で定義
enum StateId
{
  State1,
  State2,
}

// PlaceHolderState<状態識別列挙体の型>を継承する
class SampleState : PlaceHolderState<StateId>
{
  // この状態のIDを返す(必須)
  public override StateId Id => StateId.State1;
  
  // 状態が開始された際に呼び出される(任意)
  protected override void OnBegin()
  {
    // base.OnBegin() を呼び出す必要無し
  }
  
  // コンテキストのUpdateのタイミングで呼び出される(任意)
  protected override void OnUpdate()
  {
    // base.OnUpdate() を呼び出す必要無し
  }
  
  // コンテキストのLateUpdateのタイミングで呼び出される(任意)
  protected override void OnLateUpdate()
  {
    // base.OnLateUpdate() を呼び出す必要無し
  }
  
  // 状態が終了した際に呼び出される(任意)
  protected override void OnEnd()
  {
    // base.OnEnd() を呼び出す必要無し
  }
  
  // 状態変更のサンプル
  private void ChangeStateSample()
  {
    // このメソッドを呼び出すことでこの状態を管理しているステートマシンの状態が切り替わる。
    ChangeState(StateId.State2);
  }
}
```

## ステートマシンのインストール
```
class SampleInstaller : Installer
{
  public override void InstallBindings()
  {
    // BindStateMachine<状態識別列挙体の型>(開始時のステートID) の構文でステートマシンが現在のコンテキストにインストールされ、走り始める。
    Container.BindStateMachine<SampleStateId>(SampleStateId.State1);
    
    // 以下は少しテクニカルな補足
    // 現在のコンテキストにバインドされる型はIStateMachine<>,ITickable,ILateTickableの３つで、残りはサブコンテナにバインドされている。
    // ステートマシンが扱う状態はリフレクションによって収集されてサブコンテナにバインドされている。
    // 外部から状態を変更したいという特殊なパターンでない限り、IStateMachine<>を参照する必要はなく、NonLazyで自動的に走り始め、コンテキストの破棄と共に停止する。
    // コンテキストが破棄された際にすべての状態のOnDisposeが呼び出される。
    // 現在のコンテキストがサブコンテナの場合、カーネルの設定によってはステートマシンのTickが走らず更新が行われないので注意。
    // 以上のようなことを考えずに使えるように設計したので、考えるより感じて。
  }
}
```

# このプラグインのインストール
`manifest.json` に以下のように追加して UnityPackageManagerからインストールしてください。
```
{
  "dependencies": {
    "com.hiramaru.statemachine": "https://github.com/Hira-maru/StateMachineByZenject.git",
    .
    .
  }
}
```

## 依存するプラグインのインストール
Zenject(Extenject)に依存しているため、何らかの方法でインストールする必要があります。
おすすめはこちらも`manifest.json`に追加する方法です。
```
{
  "dependencies": {
    "com.hiramaru.statemachine": "https://github.com/Hira-maru/StateMachineByZenject.git",
    "com.svermeulen.extenject": "https://github.com/svermeulen/Extenject.git?path=/UnityProject/Assets/Plugins/Zenject",
    .
    .
  }
}
```
ただ、この方法は`Extenject/OptionalExtras/TestFramework`をテストアセンブリから参照できないなど、UPM特有の問題もあります。
`Extenject/OptionalExtras`を使用したい場合は素直にAssetStoreからインポートしてプロジェクトに追加するのが確実です。
