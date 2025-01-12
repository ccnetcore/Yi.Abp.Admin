interface FormItemProps {
  id: string;
  configName: string;
  configValue: string;
  configKey: string;
  configType: string;
  remark: string;
  creationTime: string;
}
interface FormProps {
  formInline: FormItemProps;
}

export type { FormItemProps, FormProps };
