// 虽然字段很少 但是抽离出来 后续有扩展字段需求就很方便了

interface FormItemProps {
  id?: string;
  /** 角色名称 */
  postName: string;
  /** 角色编号 */
  postCode: string;
  /** 备注 */
  remark: string;
  orderNum: number;
}
interface FormProps {
  formInline: FormItemProps;
}

export type { FormItemProps, FormProps };
