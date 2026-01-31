const API_BASE_URL = 'https://pastebin-production-2639.up.railway.app';

export async function createPaste(data) {
  const response = await fetch(`${API_BASE_URL}/api/pastes`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  
  if (!response.ok) {
  let errorMessage = 'Failed to create paste';
  try {
    const error = await response.json();
    errorMessage = error.error || errorMessage;
  } catch {}
  throw new Error(errorMessage);
}

  
  return response.json();
}
