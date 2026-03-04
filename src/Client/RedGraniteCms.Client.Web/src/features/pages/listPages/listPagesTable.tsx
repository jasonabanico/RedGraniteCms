import { useEffect, useRef, useState } from "react";
import { createSelector } from "reselect";
import { Container, Table, Button } from "react-bootstrap";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAppSelector, useAppDispatch } from "../../../app/hooks";
import { setPages, resetInitialLoad, deletePage } from "./listPagesTableSlice";
import pageService from "../../../services/pages";
import { makeSelectInitialLoad, makeSelectPages, makeSelectPage } from "./selectors";
import { LoadingSpinner, ErrorAlert } from "../../../components";
import { GetItems_GetItems } from "../../../services/items/__generated__/GetItems";

const pagesSelector = createSelector(makeSelectPages, (pages) => ({
    pages,
}));

const initialLoadSelector = createSelector(makeSelectInitialLoad, (initialLoad) => ({
    initialLoad,
}));

const pageSelector = createSelector(makeSelectPage, (page) => ({
    page,
}));

export function ListPagesTable() {
    const { pages } = useAppSelector(pagesSelector);
    const { initialLoad } = useAppSelector(initialLoadSelector);
    const { page } = useAppSelector(pageSelector);
    const dispatch = useAppDispatch();
    const location = useLocation();
    const navigate = useNavigate();
    const updated = location.state?.updated;
    const prevPageRef = useRef<number | undefined>(undefined);

    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [deletingId, setDeletingId] = useState<string | null>(null);

    const fetchPages = async () => {
        try {
            setIsLoading(true);
            setError(null);
            const fetchedPages = await pageService.getPages("");
            if (fetchedPages) {
                dispatch(setPages(fetchedPages));
            }
        } catch (err) {
            console.error("Error fetching pages:", err);
            setError("Failed to load pages. Please try again.");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (initialLoad && (!pages || pages.length === 0)) {
            fetchPages();
            dispatch(resetInitialLoad());
        }
        if (updated) {
            fetchPages();
            navigate(location.pathname, { replace: true });
        }
        prevPageRef.current = page;
    }, [initialLoad, pages, page, updated, dispatch, navigate, location.pathname]);

    useEffect(() => {
        if (!initialLoad && prevPageRef.current !== page) {
            fetchPages();
        }
        prevPageRef.current = page;
    }, [page, initialLoad]);

    const handleDelete = async (id: string) => {
        try {
            setDeletingId(id);
            const deleteAction = await dispatch(deletePage(id));

            if (deletePage.fulfilled.match(deleteAction)) {
                dispatch(setPages(pages?.filter((p: GetItems_GetItems) => p.id !== id) || []));
            } else {
                setError("Failed to delete page. Please try again.");
            }
        } catch (err) {
            console.error("Error deleting page:", err);
            setError("Failed to delete page. Please try again.");
        } finally {
            setDeletingId(null);
        }
    };

    if (isLoading && (!pages || pages.length === 0)) {
        return <LoadingSpinner message="Loading pages..." />;
    }

    return (
        <Container>
            {error && <ErrorAlert message={error} onDismiss={() => setError(null)} />}

            <Link to="/addPage" className='btn btn-success my-3'>Add Page</Link>

            {isLoading && <div className="text-muted mb-2">Refreshing...</div>}

            <Table className='pagesTable'>
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Slug</th>
                        <th>Summary</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {pages && pages.map((p: GetItems_GetItems) => (
                        <tr key={p.id}>
                            <td>{p.title}</td>
                            <td>{p.slug}</td>
                            <td>{p.summary}</td>
                            <td>
                                <div className="d-flex align-items-center gap-2 flex-wrap">
                                    <Link to={`/editPage/${p.id}`} className='btn bt-sm btn-primary' style={{ whiteSpace: "nowrap" }}>Edit</Link>
                                    <Button
                                        className='btn bt-sm btn-danger'
                                        style={{ whiteSpace: "nowrap" }}
                                        onClick={() => handleDelete(p.id)}
                                        disabled={deletingId === p.id}
                                    >
                                        {deletingId === p.id ? 'Deleting...' : 'Delete'}
                                    </Button>
                                </div>
                            </td>
                        </tr>
                    ))}
                    {(!pages || pages.length === 0) && !isLoading && (
                        <tr>
                            <td colSpan={4} className="text-center text-muted py-4">
                                No pages found. Click "Add Page" to create your first page.
                            </td>
                        </tr>
                    )}
                </tbody>
            </Table>
        </Container>
    );
}
