import itemService, { ItemService, ServiceError } from '../../services/items';
import type { Page, PageInput, AddPageResult, UpdatePageResult } from './types';

// Re-export types and errors only — no Item or GraphQL types
export type { Page, PageInput, AddPageResult, UpdatePageResult };
export { ServiceError };

/**
 * Page service — the only interface features use to work with pages.
 *
 * All CMS "pages" are Items on the server. This service maps between
 * Page types (what the UI sees) and Item types (what GraphQL uses).
 * Features never import from services/items/ directly.
 */
class PageService {
    constructor(private readonly items: ItemService) {}

    /** Fetch a single page by ID. */
    async getPage(id: string): Promise<Page | null> {
        if (!id) return null;
        const response = await this.items.getItem(id);
        const item = response?.GetItem;
        return item ? mapToPage(item) : null;
    }

    /** Fetch a list of pages. */
    async getPages(count = 20, skip = 0): Promise<Page[]> {
        const items = await this.items.getItems(undefined, count, skip);
        return (items ?? []).map(mapToPage);
    }

    /** Create a new page. Returns the new page's ID. */
    async addPage(input: PageInput): Promise<AddPageResult> {
        const response = await this.items.addItem({
            id: '',
            ownerId: 'admin',
            contentType: 'page',
            title: input.title,
            slug: input.slug,
            summary: input.summary,
            content: input.content,
        });
        return { id: response.AddItem?.id ?? '' };
    }

    /** Update an existing page. Returns the updated page's ID. */
    async updatePage(input: PageInput): Promise<UpdatePageResult> {
        if (!input.id) {
            throw new ServiceError('Page ID is required for updates.', 'VALIDATION_ERROR');
        }
        const response = await this.items.updateItem({
            id: input.id,
            ownerId: 'admin',
            contentType: 'page',
            title: input.title,
            slug: input.slug,
            summary: input.summary,
            content: input.content,
        });
        return { id: response.UpdateItem?.id ?? '' };
    }

    /** Delete a page by ID. */
    async deletePage(id: string): Promise<boolean> {
        return this.items.deleteItem(id);
    }
}

/** Map a GraphQL Item response to a Page. */
function mapToPage(item: { id: string; title: string; slug: string | null; summary: string | null; content?: string | null }): Page {
    return {
        id: item.id,
        title: item.title,
        slug: item.slug,
        summary: item.summary,
        content: item.content ?? null,
    };
}

export function createPageService(items: ItemService = itemService): PageService {
    return new PageService(items);
}

const pageService = createPageService();
export default pageService;
