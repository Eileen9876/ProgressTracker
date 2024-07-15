# 進度追蹤視窗

## 程式執行結果展示

正確執行

<image src="https://github.com/user-attachments/assets/a2882936-d07d-4ed1-b793-4f03592b8346" width="60%">

使用者點選取消

<image src="https://github.com/user-attachments/assets/205f5821-75d4-42c1-bbc2-ab306d78678d" width="60%">

## 使用方法

方法一：
```csharp
using mylib;

//建立進度追蹤
ProgressTrack track = new ProgressTrack(this); 

//執行並顯示追蹤視窗
track.Run(() =>
{
    track.SetMsg("載入資料 ..."); //於視窗中顯示訊息
    LoadData();
    track.CancelCheck(); //確認使用者是否按下【取消】，假如按下則中斷並跳出。

    track.SetMsg("處理中 ..."); 
    ProcData();
    track.CancelCheck(); 
});
```

方法二：
```csharp
using mylib;

ProgressTrack track = ProgressTrack.Run(this, () =>
{
    track.SetMsg("載入資料 ..."); //於視窗中顯示訊息
    LoadData();
    track.CancelCheck(); //確認使用者是否按下【取消】，假如按下則中斷並跳出。

    track.SetMsg("處理中 ..."); 
    ProcData();
    track.CancelCheck(); 
});
```

