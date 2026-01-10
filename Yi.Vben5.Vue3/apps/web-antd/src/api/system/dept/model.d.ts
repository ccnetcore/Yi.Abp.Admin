export interface Dept {
  creationTime: string;
  creatorId?: string | null;
  state: boolean;
  deptName: string;
  deptCode?: string;
  leader?: string;
  leaderName?: string;
  parentId: string | null;
  remark?: string;
  orderNum: number;
  id: string;
  children?: Dept[];
}
