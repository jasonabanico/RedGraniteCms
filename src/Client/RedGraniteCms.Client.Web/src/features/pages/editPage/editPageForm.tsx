import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { z } from 'zod';
import { ItemInput } from '../../../../__generated__/globalTypes';
import { useAppDispatch } from '../../../app/hooks';
import { updatePage } from './editPageFormSlice';
import pageService from '../../../services/pages';
import { LoadingSpinner, ErrorAlert } from '../../../components';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';

// Validation schema
const pageSchema = z.object({
    title: z.string().min(1, 'Title is required').max(500, 'Title must not exceed 500 characters'),
    slug: z.string().regex(/^[a-z0-9]+(?:-[a-z0-9]+)*$/, 'Slug must be lowercase alphanumeric words separated by hyphens').optional().or(z.literal('')),
    summary: z.string().max(2000, 'Summary must not exceed 2000 characters').optional().or(z.literal('')),
    content: z.string().max(50000, 'Content must not exceed 50000 characters').optional().or(z.literal('')),
});

interface ValidationErrors {
    title?: string;
    slug?: string;
    summary?: string;
    content?: string;
}

export function EditPageForm() {
    const [title, setTitle] = useState('');
    const [slug, setSlug] = useState('');
    const [summary, setSummary] = useState('');
    const [content, setContent] = useState('');
    const [isLoading, setIsLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});

    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const { pageId } = useParams<{ pageId: string }>();

    useEffect(() => {
        async function fetchPageDetails() {
            if (!pageId) {
                setError('Page ID is required');
                setIsLoading(false);
                return;
            }

            try {
                setIsLoading(true);
                setError(null);
                const response = await pageService.getPage(pageId);
                const pageDetails = response?.GetItem;

                if (pageDetails) {
                    setTitle(pageDetails.title);
                    setSlug(pageDetails.slug || '');
                    setSummary(pageDetails.summary || '');
                    setContent(pageDetails.content || '');
                } else {
                    setError('Page not found');
                }
            } catch (err) {
                console.error('Error fetching page:', err);
                setError('Failed to load page details. Please try again.');
            } finally {
                setIsLoading(false);
            }
        }

        fetchPageDetails();
    }, [pageId]);

    const validateForm = (): boolean => {
        const result = pageSchema.safeParse({ title, slug: slug || undefined, summary: summary || undefined, content: content || undefined });

        if (!result.success) {
            const errors: ValidationErrors = {};
            result.error.errors.forEach((err) => {
                const field = err.path[0] as keyof ValidationErrors;
                errors[field] = err.message;
            });
            setValidationErrors(errors);
            return false;
        }

        setValidationErrors({});
        return true;
    };

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();

        if (!validateForm()) {
            return;
        }

        const itemInput: ItemInput = {
            id: pageId || '',
            title,
            slug: slug || undefined,
            summary: summary || undefined,
            content: content || undefined,
        };

        try {
            setIsSubmitting(true);
            setError(null);

            const resultAction = await dispatch(updatePage(itemInput));

            if (updatePage.rejected.match(resultAction)) {
                throw new Error('Failed to update page');
            }

            navigate('/', { state: { updated: true } });
        } catch (err) {
            console.error('Error updating page:', err);
            setError('Failed to update page. Please try again.');
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleCancel = () => {
        navigate('/');
    };

    if (isLoading) {
        return <LoadingSpinner message="Loading page details..." />;
    }

    return (
        <div className="container mx-auto max-w-2xl px-4">
            <h2 className="text-2xl font-bold mb-4">Edit Page</h2>

            {error && <ErrorAlert message={error} onDismiss={() => setError(null)} />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                    <Label htmlFor="title">Title</Label>
                    <Input
                        id="title"
                        type="text"
                        name="title"
                        placeholder="Enter title"
                        value={title}
                        onChange={e => setTitle(e.target.value)}
                        disabled={isSubmitting}
                        className={validationErrors.title ? 'border-destructive' : ''}
                    />
                    {validationErrors.title && (
                        <p className="text-sm text-destructive">{validationErrors.title}</p>
                    )}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="slug">Slug</Label>
                    <Input
                        id="slug"
                        type="text"
                        name="slug"
                        placeholder="e.g. my-first-page"
                        value={slug}
                        onChange={e => setSlug(e.target.value)}
                        disabled={isSubmitting}
                        className={validationErrors.slug ? 'border-destructive' : ''}
                    />
                    <p className="text-sm text-muted-foreground">
                        URL-safe identifier (lowercase, hyphens only).
                    </p>
                    {validationErrors.slug && (
                        <p className="text-sm text-destructive">{validationErrors.slug}</p>
                    )}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="summary">Summary</Label>
                    <Textarea
                        id="summary"
                        rows={2}
                        name="summary"
                        placeholder="Brief summary for listings and SEO"
                        value={summary}
                        onChange={e => setSummary(e.target.value)}
                        disabled={isSubmitting}
                        className={validationErrors.summary ? 'border-destructive' : ''}
                    />
                    {validationErrors.summary && (
                        <p className="text-sm text-destructive">{validationErrors.summary}</p>
                    )}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="content">Content</Label>
                    <Textarea
                        id="content"
                        rows={8}
                        name="content"
                        placeholder="Page content (HTML or Markdown)"
                        value={content}
                        onChange={e => setContent(e.target.value)}
                        disabled={isSubmitting}
                        className={validationErrors.content ? 'border-destructive' : ''}
                    />
                    {validationErrors.content && (
                        <p className="text-sm text-destructive">{validationErrors.content}</p>
                    )}
                </div>

                <div className="flex gap-2">
                    <Button type="submit" disabled={isSubmitting}>
                        {isSubmitting ? 'Saving...' : 'Submit'}
                    </Button>
                    <Button variant="secondary" type="button" onClick={handleCancel} disabled={isSubmitting}>
                        Cancel
                    </Button>
                </div>
            </form>
        </div>
    );
}
