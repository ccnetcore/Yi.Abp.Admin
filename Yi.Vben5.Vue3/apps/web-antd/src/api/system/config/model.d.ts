export interface SysConfig {
  id: string;
  configName: string;
  configKey: string;
  configValue: string;
  configType: string | null;
  orderNum: number;
  remark: string | null;
  isDeleted: boolean;
  creationTime: string;
  creatorId: string | null;
  lastModifierId: string | null;
  lastModificationTime: string | null;
}
