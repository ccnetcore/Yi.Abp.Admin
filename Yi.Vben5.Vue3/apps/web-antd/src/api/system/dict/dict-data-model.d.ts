export interface DictData {
  id: string;
  isDeleted: boolean;
  orderNum: number;
  state: boolean;
  remark: string | null;
  listClass: string | null;
  cssClass: string | null;
  dictType: string;
  dictLabel: string | null;
  dictValue: string;
  isDefault: boolean;
  creationTime: string;
  creatorId: string | null;
  lastModifierId: string | null;
  lastModificationTime: string | null;
}
