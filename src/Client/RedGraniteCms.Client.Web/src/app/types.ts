import { IAddPageFormState } from "../features/pages/addPage/types";
import { IEditPageFormState } from "../features/pages/editPage/types";
import { IListPagesTableState } from "../features/pages/listPages/types";

export interface IRootState {
  listPagesTable: IListPagesTableState;
  addPageForm: IAddPageFormState;
  editPageForm: IEditPageFormState;
}