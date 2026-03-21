import { useEffect, useRef, useState } from "react";
import { createSelector } from "reselect";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAppSelector, useAppDispatch } from "../../../app/hooks";
import { setPages, resetInitialLoad, deletePage } from "./listPagesTableSlice";
import pageService from "../../../services/pages";
import { makeSelectInitialLoad, makeSelectPages, makeSelectPage } from "./selectors";
import { LoadingSpinner, ErrorAlert } from "../../../components";
import { GetItems_GetItems } from "../../../services/items/__generated__/GetItems";
import { Button } from "@/components/ui/button";
import {
    Table,
    TableHeader,
    TableBody,
    TableRow,
    TableHead,
    TableCell,
} from "@/components/ui/table";

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
        <div className="container mx-auto max-w-4xl px-4">
            {error && <ErrorAlert message={error} onDismiss={() => setError(null)} />}

            <Button asChild variant="success" className="my-3">
                <Link to="/addPage">Add Page</Link>
            </Button>

            {isLoading && <div className="text-sm text-muted-foreground mb-2">Refreshing...</div>}

            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead>Title</TableHead>
                        <TableHead>Slug</TableHead>
                        <TableHead>Summary</TableHead>
                        <TableHead>Actions</TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {pages && pages.map((p: GetItems_GetItems) => (
                        <TableRow key={p.id}>
                            <TableCell>{p.title}</TableCell>
                            <TableCell>{p.slug}</TableCell>
                            <TableCell>{p.summary}</TableCell>
                            <TableCell>
                                <div className="flex items-center gap-2 flex-wrap">
                                    <Button asChild size="sm">
                                        <Link to={`/editPage/${p.id}`}>Edit</Link>
                                    </Button>
                                    <Button
                                        variant="destructive"
                                        size="sm"
                                        onClick={() => handleDelete(p.id)}
                                        disabled={deletingId === p.id}
                                    >
                                        {deletingId === p.id ? 'Deleting...' : 'Delete'}
                                    </Button>
                                </div>
                            </TableCell>
                        </TableRow>
                    ))}
                    {(!pages || pages.length === 0) && !isLoading && (
                        <TableRow>
                            <TableCell colSpan={4} className="text-center text-muted-foreground py-4">
                                No pages found. Click "Add Page" to create your first page.
                            </TableCell>
                        </TableRow>
                    )}
                </TableBody>
            </Table>
        </div>
    );
}
