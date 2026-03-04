import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import pageService from '../../../services/pages';
import { IListPagesTableState } from './types';

const initialState: IListPagesTableState = {
    initialLoad: true,
    page: 1,
    pages: [],
}

export const deletePage = createAsyncThunk(
    'pagesTable/deletePage',
    async (id: string, { rejectWithValue }) => {
        try {
            await pageService.deletePage(id);
            return id;
        } catch (err: any) {
            return rejectWithValue(err.response.data);
        }
    }
);

const listPagesTableSlice = createSlice({
    name: 'pagesTable',
    initialState,
    reducers: {
        setPages: (state, action) => {
            state.pages = action.payload;
            state.initialLoad = false;
        },
        setPage: (state, action) => {
            state.page = action.payload
        },
        resetInitialLoad(state) {
            state.initialLoad = false;
        },
        savePage: (state, action) => {
            state.pages?.unshift(action.payload);
        }
    }
});

export const { setPages, setPage, resetInitialLoad, savePage } = listPagesTableSlice.actions;
export default listPagesTableSlice.reducer;
