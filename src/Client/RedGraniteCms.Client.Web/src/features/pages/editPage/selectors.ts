import { createSelector } from "reselect";
import { IRootState } from "../../../app/types";

const selectEditPageForm = (state: IRootState) => state.editPageForm;

export const makeSelectPage = createSelector(
    selectEditPageForm,
    (editPageForm) => editPageForm.page
);
