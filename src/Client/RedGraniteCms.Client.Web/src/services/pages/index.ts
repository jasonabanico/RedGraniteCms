import itemService, { ItemService } from '../items';

// Re-export core types so module consumers don't need to reach into items/
export type { GetItem, GetItem_GetItem } from '../items/__generated__/GetItem';
export type { GetItems, GetItems_GetItems } from '../items/__generated__/GetItems';
export type { AddItem, AddItem_AddItem } from '../items/__generated__/AddItem';
export type { UpdateItem, UpdateItem_UpdateItem } from '../items/__generated__/UpdateItem';
export type { DeleteItem } from '../items/__generated__/DeleteItem';
export { ServiceError } from '../items';

/**
 * Page service — a module-level facade over the core ItemService.
 *
 * All CMS "pages" are Items on the server. This wrapper lets the Pages
 * feature speak in page terminology while delegating to the shared
 * Item infrastructure. Future modules (UserProfile, BlogPost, etc.)
 * follow the same pattern.
 */
class PageService {
  constructor(private readonly items: ItemService) {}

  /** Fetch a single page by ID. */
  getPage(id: string | undefined) {
    return this.items.getItem(id);
  }

  /** Fetch a list of pages. */
  getPages(isoDateString: string, count = 20) {
    return this.items.getItems(isoDateString, count);
  }

  /** Create a new page. */
  addPage(item: Parameters<ItemService['addItem']>[0]) {
    return this.items.addItem(item);
  }

  /** Update an existing page. */
  updatePage(item: Parameters<ItemService['updateItem']>[0]) {
    return this.items.updateItem(item);
  }

  /** Delete a page by ID. */
  deletePage(id: string) {
    return this.items.deleteItem(id);
  }
}

export function createPageService(items: ItemService = itemService): PageService {
  return new PageService(items);
}

const pageService = createPageService();
export default pageService;
