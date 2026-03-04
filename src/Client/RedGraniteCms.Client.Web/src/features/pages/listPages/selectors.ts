import { createSelector } from "reselect";
import { IRootState } from "../../../app/types";

const listPagesTable = (state: IRootState) => state.listPagesTable;

export const makeSelectPages = createSelector(
    listPagesTable,
    (listPagesTable) => listPagesTable.pages
);

export const makeSelectInitialLoad = createSelector(
    listPagesTable,
    (listPagesTable) => listPagesTable.initialLoad
);

export const makeSelectPage = createSelector(
    listPagesTable,
    (listPagesTable) => listPagesTable.page
);
