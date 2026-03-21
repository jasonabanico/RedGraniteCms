/**
 * Types for the Pages module.
 * Features import these — never Item or GraphQL types directly.
 */

/** The possible statuses of a page. */
export type PageStatus = 'Draft' | 'Published' | 'Archived';

/** A page as seen by the UI — mapped from server Item. */
export interface Page {
    id: string;
    title: string;
    slug: string | null;
    summary: string | null;
    content: string | null;
    status: PageStatus;
}

/** Input for creating or updating a page. */
export interface PageInput {
    id?: string;
    title: string;
    slug?: string;
    summary?: string;
    content?: string;
    status?: PageStatus;
}

/** Result of adding a page — contains the new page's ID. */
export interface AddPageResult {
    id: string;
}

/** Result of updating a page — contains the updated page's ID. */
export interface UpdatePageResult {
    id: string;
}
