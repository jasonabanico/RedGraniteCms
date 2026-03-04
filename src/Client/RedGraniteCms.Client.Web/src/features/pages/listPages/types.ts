import { GetItems } from "../../../services/items/__generated__/GetItems";

export interface IListPagesTableState {
    initialLoad: boolean;
    page: number;
    pages: GetItems["GetItems"] | null;
}
