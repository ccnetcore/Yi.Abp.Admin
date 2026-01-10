export interface DictType {
  id: string;
  isDeleted: boolean;
  orderNum: number;
  state: boolean | null;
  dictName: string;
  dictType: string;
  remark: string | null;
  creationTime: string;
  creatorId: string | null;
  lastModifierId: string | null;
  lastModificationTime: string | null;
}
