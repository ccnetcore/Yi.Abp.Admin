interface FormItemProps {
  id: string;
  higherDeptOptions: Record<string, unknown>[];
  parentId: string;
  deptName: string;
  leader: string;
  phone: string | number;
  email: string;
  orderNum: number;
  state: boolean;
  remark: string;
  deptCode: string;
}
interface FormProps {
  formInline: FormItemProps;
}

export type { FormItemProps, FormProps };
