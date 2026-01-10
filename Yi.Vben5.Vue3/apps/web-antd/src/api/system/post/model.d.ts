/**
 * @description: Post interface
 */
export interface Post {
  id: string;
  isDeleted: boolean;
  creationTime: string;
  creatorId: string | null;
  lastModifierId: string | null;
  lastModificationTime: string | null;
  orderNum: number;
  state: boolean;
  postCode: string;
  postName: string;
  deptId: string;
  remark: string | null;
}
