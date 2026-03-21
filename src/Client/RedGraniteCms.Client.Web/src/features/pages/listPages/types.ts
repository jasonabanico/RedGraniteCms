import type { Page } from "../../../modules/pages/types";

export interface IListPagesTableState {
    initialLoad: boolean;
    page: number;
    pages: Page[] | null;
}
