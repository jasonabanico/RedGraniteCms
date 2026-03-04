import {
  configureStore,
  ThunkAction,
  Action,
  combineReducers
} from "@reduxjs/toolkit";
import listPagesTableReducer from "../features/pages/listPages/listPagesTableSlice";
import addPageFormReducer from "../features/pages/addPage/addPageFormSlice";
import editPageFormReducer from '../features/pages/editPage/editPageFormSlice';
import ReduxLogger from "redux-logger";

const rootReducer = combineReducers({
  listPagesTable: listPagesTableReducer,
  addPageForm: addPageFormReducer,
  editPageForm: editPageFormReducer,
});

export const store = configureStore({
  middleware:(getDefaultMiddleware) =>
    getDefaultMiddleware().concat(ReduxLogger),
  reducer: rootReducer,
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;