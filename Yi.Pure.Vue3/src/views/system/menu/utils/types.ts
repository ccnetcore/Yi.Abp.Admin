interface FormItemProps {
  /** 菜单类型（0目录、1代表菜单、2代表组件*/
  id?: string;
  menuType: number | string;
  higherMenuOptions: Record<string, unknown>[];
  parentId: string;
  menuName: string;
  router: string;
  component: string;
  orderNum: number;
  menuIcon: string;
  permissionCode: string;
  isShow: boolean;
  isLink: boolean;
  state: boolean;
  routerName: string;
  menuSource: string;
}
interface FormProps {
  formInline: FormItemProps;
}

export type { FormItemProps, FormProps };
