import type { Ref } from 'vue';

import { computed, ref, unref } from 'vue';

/**
 * Paginates an array of items
 * @param list The array to paginate
 * @param pageNo The current page number (1-based)
 * @param MaxResultCount Number of items per page
 * @returns Paginated array slice
 * @throws {Error} If pageNo or MaxResultCount are invalid
 */
function pagination<T = any>(list: T[], pageNo: number, MaxResultCount: number): T[] {
  if (pageNo < 1) throw new Error('Page number must be positive');
  if (MaxResultCount < 1) throw new Error('Page size must be positive');

  const offset = (pageNo - 1) * Number(MaxResultCount);
  const ret =
    offset + MaxResultCount >= list.length
      ? list.slice(offset)
      : list.slice(offset, offset + MaxResultCount);
  return ret;
}

export function usePagination<T = any>(list: Ref<T[]>, MaxResultCount: number) {
  const currentPage = ref(1);
  const MaxResultCountRef = ref(MaxResultCount);

  const totalPages = computed(() =>
    Math.ceil(unref(list).length / unref(MaxResultCountRef)),
  );

  const paginationList = computed(() => {
    return pagination(unref(list), unref(currentPage), unref(MaxResultCountRef));
  });

  const total = computed(() => {
    return unref(list).length;
  });

  function setCurrentPage(page: number) {
    if (page < 1 || page > unref(totalPages)) {
      throw new Error('Invalid page number');
    }
    currentPage.value = page;
  }

  function setMaxResultCount(MaxResultCount: number) {
    if (MaxResultCount < 1) {
      throw new Error('Page size must be positive');
    }
    MaxResultCountRef.value = MaxResultCount;
    // Reset to first page to prevent invalid state
    currentPage.value = 1;
  }

  return { setCurrentPage, total, setMaxResultCount, paginationList };
}
