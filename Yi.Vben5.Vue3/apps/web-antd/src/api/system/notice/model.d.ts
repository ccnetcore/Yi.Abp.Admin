export interface Notice {
  id: string;
  title: string;
  type: string;
  content: string;
  state: boolean;
  isDeleted: boolean;
  creationTime: string;
  creatorId?: string | null;
  lastModifierId?: string | null;
  lastModificationTime?: string | null;
  orderNum: number;
}
