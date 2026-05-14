## Assessment Extraction

### ── 1. DETECTION ──────────────────────────────────────────────────────────

- Determines whether a PDF has usable embedded text or is a scanned image.
-  This is instantaneous — no API calls, no cost.

### ── 2. FULL DOCUMENT EXTRACTION ───────────────────────────────────────────

- Extracts structured content from a digital PDF.
- Returns a PdfExtractionResult with per-page blocks ready for Gemini structuring.

### ── 3. PAGE-LEVEL EXTRACTION ──────────────────────────────────────────────

- ContentOrderTextExtractor respects the visual reading order of the page —
- top-to-bottom, left-to-right. This is important for numbered exam questions
- where order must be preserved.
 
### ── 4. TEXT BLOCK CONSTRUCTION ────────────────────────────────────────────

- Groups words into logical text blocks by proximity.
- Helps Gemini understand paragraph and question boundaries
- even when the prompt passes plain text.
 
### ── 5. TABLE DETECTION HEURISTIC ─────────────────────────────────────────

- Heuristic: if many words share very similar Y positions in distinct vertical bands,
- the page likely contains a table. Flag it so the caller can decide whether to
- send this page to Gemini vision instead of relying on text extraction alone.
- Tables in exam papers are rare but do appear in data/science questions.

### ── 6. TEXT NORMALIZATION ─────────────────────────────────────────────────

- Cleans up common PdfPig extraction artifacts before passing text to Gemini:
- Collapses excessive whitespace
- Normalizes Unicode ligatures (ﬁ → fi, ﬂ → fl, etc.)
- Removes page header/footer noise (page numbers, repeated titles)
- Preserves question numbering patterns
