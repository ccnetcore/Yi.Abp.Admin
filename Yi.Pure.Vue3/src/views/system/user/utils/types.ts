interface FormItemProps {
  id?: string;
  /** 用于判断是`新增`还是`修改` */
  title: string;
  higherDeptOptions: Record<string, unknown>[];
  deptId?: string;
  nick: string;
  userName: string;
  password: string;
  phone: string | number;
  email: string;
  sex: string | number;
  state: boolean;
  deptName?: string;
  remark: string;

  roleIds: string[];
  roleOptions: any[];
}
interface FormProps {
  formInline: FormItemProps;
}

interface RoleFormItemProps {
  userName: string;
  nick: string;
  /** 角色列表 */
  roleOptions: any[];
  /** 选中的角色列表 */
  ids: Record<string, unknown>[];
}
interface RoleFormProps {
  formInline: RoleFormItemProps;
}

export type { FormItemProps, FormProps, RoleFormItemProps, RoleFormProps };
