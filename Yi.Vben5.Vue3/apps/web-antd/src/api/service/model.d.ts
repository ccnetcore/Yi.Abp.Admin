export interface CpuInfo {
  coreTotal: number;
  logicalProcessors: number;
  cpuRate: number;
}

export interface MemoryInfo {
  totalRAM: string;
  usedRam: string;
  freeRam: string;
  ramRate: number;
}

export interface SystemInfo {
  computerName: string;
  osName: string;
  serverIP: string;
  osArch: string;
}

export interface AppInfo {
  name: string;
  version: string;
  startTime: string;
  runTime: string;
  rootPath: string;
  webRootPath: string;
}

export interface DiskInfo {
  diskName: string;
  typeName: string;
  totalSize: string;
  availableFreeSpace: string;
  used: string;
  availablePercent: number;
}

export interface ServerInfo {
  cpu: CpuInfo;
  memory: MemoryInfo;
  sys: SystemInfo;
  app: AppInfo;
  disk: DiskInfo[];
}

