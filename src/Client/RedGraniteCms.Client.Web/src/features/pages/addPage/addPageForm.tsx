import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Button } from 'react-bootstrap';
import Form from 'react-bootstrap/Form';
import { z } from 'zod';
import { ItemInput } from '../../../../__generated__/globalTypes';
import { useAppDispatch } from '../../../app/hooks';
import { AddItem } from '../../../services/items/__generated__/AddItem';
import { savePage } from '../listPages/listPagesTableSlice';
import { addPage } from './addPageFormSlice';
import { ErrorAlert } from '../../../components';

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

export function AddPageForm() {
    const [title, setTitle] = useState('');
    const [slug, setSlug] = useState('');
    const [summary, setSummary] = useState('');
    const [content, setContent] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});

    const dispatch = useAppDispatch();
    const navigate = useNavigate();

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
            id: "",
            title,
            slug: slug || undefined,
            summary: summary || undefined,
            content: content || undefined,
        };

        try {
            setIsSubmitting(true);
            setError(null);

            const savedPageAction = await dispatch(addPage(itemInput));

            if (addPage.rejected.match(savedPageAction)) {
                throw new Error('Failed to add page');
            }

            const savedPage = savedPageAction.payload as AddItem;
            itemInput.id = savedPage.AddItem ? savedPage.AddItem.id : "";
            dispatch(savePage(itemInput));
            navigate('/');
        } catch (err) {
            console.error('Error adding page:', err);
            setError('Failed to add page. Please try again.');
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleCancel = () => {
        navigate('/');
    };

    return (
        <Container>
            <h2>Add Page</h2>

            {error && <ErrorAlert message={error} onDismiss={() => setError(null)} />}

            <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3" controlId="formPageTitle">
                    <Form.Label>Title</Form.Label>
                    <Form.Control
                        type="text"
                        name="title"
                        placeholder="Enter title"
                        value={title}
                        onChange={e => setTitle(e.target.value)}
                        isInvalid={!!validationErrors.title}
                        disabled={isSubmitting}
                    />
                    <Form.Control.Feedback type="invalid">
                        {validationErrors.title}
                    </Form.Control.Feedback>
                </Form.Group>

                <Form.Group className="mb-3" controlId="formPageSlug">
                    <Form.Label>Slug</Form.Label>
                    <Form.Control
                        type="text"
                        name="slug"
                        placeholder="e.g. my-first-page"
                        value={slug}
                        onChange={e => setSlug(e.target.value)}
                        isInvalid={!!validationErrors.slug}
                        disabled={isSubmitting}
                    />
                    <Form.Text className="text-muted">
                        URL-safe identifier (lowercase, hyphens only). Leave blank to auto-generate.
                    </Form.Text>
                    <Form.Control.Feedback type="invalid">
                        {validationErrors.slug}
                    </Form.Control.Feedback>
                </Form.Group>

                <Form.Group className="mb-3" controlId="formPageSummary">
                    <Form.Label>Summary</Form.Label>
                    <Form.Control
                        as="textarea"
                        rows={2}
                        name="summary"
                        placeholder="Brief summary for listings and SEO"
                        value={summary}
                        onChange={e => setSummary(e.target.value)}
                        isInvalid={!!validationErrors.summary}
                        disabled={isSubmitting}
                    />
                    <Form.Control.Feedback type="invalid">
                        {validationErrors.summary}
                    </Form.Control.Feedback>
                </Form.Group>

                <Form.Group className="mb-3" controlId="formPageContent">
                    <Form.Label>Content</Form.Label>
                    <Form.Control
                        as="textarea"
                        rows={8}
                        name="content"
                        placeholder="Page content (HTML or Markdown)"
                        value={content}
                        onChange={e => setContent(e.target.value)}
                        isInvalid={!!validationErrors.content}
                        disabled={isSubmitting}
                    />
                    <Form.Control.Feedback type="invalid">
                        {validationErrors.content}
                    </Form.Control.Feedback>
                </Form.Group>

                <Button variant="primary" type="submit" disabled={isSubmitting}>
                    {isSubmitting ? 'Saving...' : 'Submit'}
                </Button>
                <Button variant="secondary" onClick={handleCancel} className="mx-2" disabled={isSubmitting}>
                    Cancel
                </Button>
            </Form>
        </Container>
    );
}
