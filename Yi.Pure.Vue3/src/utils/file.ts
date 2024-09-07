import { isAllEmpty } from "@pureadmin/utils";

export function getFileUrl(fileId: string, tryPath: string): string {
  if (isAllEmpty(fileId)) {
    return tryPath;
  }
  return `${import.meta.env.VITE_APP_BASE_API}/file/${fileId}`;
}
