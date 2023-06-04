# KTO-2508 memory leak


事前安裝
```
curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh -c 6.0 --install-dir /usr/share/dotnet
export PATH=$PATH:$HOME/.dotnet/tools
```
```
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace
dotnet tool install --global dotnet-dump
dotnet tool install --global dotnet-monitor
dotnet tool install --global dotnet-gcdump
```

起因
![](https://i.imgur.com/aI0GjHs.png)
![](https://i.imgur.com/5tHfWo7.png)
![](https://i.imgur.com/Xa6Bf4G.png)

find the pid
```
dotnet-counters ps 
supervisorctl status dotnet
```
使用dotnet monitor查看RunTime中的資訊
```
dotnet-counters monitor --refresh-interval 1 -p {pid}
```
例如以這個範例來說，可以看到有非常大量的記憶體停留在Gen2
![](https://hackmd.io/_uploads/BkSkshk8n.png)


產生dump檔
```
dotnet-dump collect -p {pid}
```

分析dump檔
```
dotnet-dump analyze {fileName}
```

查看目前受託館Heap的整體狀態
```
dumpheap -stat
dumpheap -stat -type 'Jaeger'
```
尋找物件的位址
```
dumpheap -mt 00007faddaa50f90
```
列出物件的屬性
```
dumpobj {Address}
```


進入分析的模式後可以輸入一下指令查看相關訊息
```
threadpool
```

其他工具:

## WinDBG

```

!analyze -v
!address -summary
!dumpheap -stat
```

## DotMemory
可直接分析Dump檔
也可以直接Attach process
```
apt-get update -y && apt-get install -y wget && \
wget -O dotMemoryclt.zip https://www.nuget.org/api/v2/package/JetBrains.dotMemory.Console.linux-x64 && \
apt-get install -y unzip && \
unzip dotMemoryclt.zip -d ./dotMemoryclt && \
chmod +x -R dotMemoryclt/*
```
get the process ID:
```
dotnet-counters ps 
supervisorctl status dotnet
```
get snapshot
```
./dotMemoryclt/tools/dotmemory get-snapshot 1

./dotMemoryclt/tools/dotmemory attach 1
##dotMemory["get-snapshot"]
```
Analyze the collected snapshots.
```
```
reference

https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap

https://learn.microsoft.com/en-us/dotnet/core/diagnostics/?WT.mc_id=DT-MVP-4015686