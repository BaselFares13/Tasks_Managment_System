const API = 'http://localhost:5252/api/tasks';

const PRIORITY_LABEL = { 0: 'Low', 1: 'Medium', 2: 'High' };
const STATUS_LABEL = { 0: 'ToDo', 1: 'Completed' };


function toLocalDatetimeValue(dateStr) {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toISOString().slice(0, 16);
}


async function apiFetch(url, options = {}) {
    const res = await fetch(url, {
        headers: { 'Content-Type': 'application/json' },
        ...options
    });
    if (res.status === 204) return null;
    return res.json();
}

function renderTasks(tasks) {
    const list = document.getElementById('taskList');
    if (!tasks || tasks.length === 0) {
        list.innerHTML = '<p class="empty">No tasks found.</p>';
        return;
    }

    list.innerHTML = tasks.map(t => {
        const pLabel = t.priority;
        const sLabel = t.status;
        const isCompleted = sLabel === 'Completed';
        const deadline = t.deadline ? new Date(t.deadline).toLocaleString() : '—';

        return `
      <div class="task-card priority-${pLabel} ${isCompleted ? 'completed' : ''}">
        <div>
          <div class="task-title">${t.title}</div>
          <div class="task-desc">${t.description}</div>
          <div class="task-meta">
            <span>📅 ${deadline}</span>
            <span class="badge badge-${pLabel}">${pLabel}</span>
            <span class="badge badge-${sLabel}">${sLabel}</span>
          </div>
        </div>
        <div class="task-actions">
          ${!isCompleted ? `<button class="btn-complete" onclick="markCompleted('${t.id}')">✓ Done</button>` : ''}
          <button class="btn-edit" onclick="openEdit('${t.id}')">Edit</button>
          <button class="btn-delete" onclick="deleteTask('${t.id}')">Delete</button>
        </div>
      </div>
    `;
    }).join('');
}


async function loadTasks() {
    const filterCompleted = document.getElementById('filterCompleted').checked;
    const filterIncompleted = document.getElementById('filterIncompleted').checked;
    const filterHigh = document.getElementById('filterHighPriority').checked;
    const sortOrder = document.querySelector('input[name="sortOrder"]:checked').value;

    try {
        let data;

        if (filterCompleted || filterHigh || filterIncompleted) {
            const params = new URLSearchParams({
                completed: filterCompleted,
                highPrioritized: filterHigh,
                incompleted: filterIncompleted
            });
            data = await apiFetch(`${API}/Filter?${params}`);
        } else {
            const params = new URLSearchParams();
            if (sortOrder === 'deadline') params.set('orderByDeadline', true);
            if (sortOrder === 'priority') params.set('orderByPriority', true);
            data = await apiFetch(`${API}?${params}`);
        }

        renderTasks(data?.data ?? []);
    } catch (err) {
        document.getElementById('taskList').innerHTML = `<p class="empty">Failed to load tasks.</p>`;
    }
}

document.getElementById('openAddModal').addEventListener('click', () => {
    document.getElementById('addModal').classList.remove('hidden');
    document.getElementById('addError').textContent = '';
});

document.getElementById('cancelAdd').addEventListener('click', () => {
    document.getElementById('addModal').classList.add('hidden');
});

document.getElementById('submitAdd').addEventListener('click', async () => {
    const title = document.getElementById('addTitle').value.trim();
    const desc = document.getElementById('addDesc').value.trim();
    const deadline = document.getElementById('addDeadline').value;
    const priority = parseInt(document.getElementById('addPriority').value);
    const errEl = document.getElementById('addError');

    if (!title || !desc || !deadline) {
        errEl.textContent = 'All fields are required.';
        return;
    }

    const body = { title, description: desc, deadline: new Date(deadline).toISOString(), priority };
    const res = await apiFetch(API, { method: 'POST', body: JSON.stringify(body) });

    if (res?.success) {
        document.getElementById('addModal').classList.add('hidden');
        document.getElementById('addTitle').value = '';
        document.getElementById('addDesc').value = '';
        document.getElementById('addDeadline').value = '';
        loadTasks();
    } else {
        errEl.textContent = res?.message || 'Error creating task.';
    }
});

async function openEdit(id) {
    const res = await apiFetch(`${API}/${id}`);
    if (!res?.success) return alert('Could not load task.');

    const t = res.data;
    const pLabel = t.priority;
    const sLabel =t.status;

    document.getElementById('editId').value = t.id;
    document.getElementById('editTitle').value = t.title;
    document.getElementById('editDesc').value = t.description;
    document.getElementById('editDeadline').value = toLocalDatetimeValue(t.deadline);

    document.getElementById('editPriority').value =
        Object.entries(PRIORITY_LABEL).find(([, v]) => v === pLabel)?.[0] ?? 1;
    document.getElementById('editStatus').value =
        Object.entries(STATUS_LABEL).find(([, v]) => v === sLabel)?.[0] ?? 0;

    document.getElementById('editError').textContent = '';
    document.getElementById('editModal').classList.remove('hidden');
}

document.getElementById('cancelEdit').addEventListener('click', () => {
    document.getElementById('editModal').classList.add('hidden');
});

document.getElementById('submitEdit').addEventListener('click', async () => {
    const id = document.getElementById('editId').value;
    const title = document.getElementById('editTitle').value.trim();
    const desc = document.getElementById('editDesc').value.trim();
    const deadline = document.getElementById('editDeadline').value;
    const priority = parseInt(document.getElementById('editPriority').value);
    const status = parseInt(document.getElementById('editStatus').value);
    const errEl = document.getElementById('editError');

    if (!title || !desc || !deadline) {
        errEl.textContent = 'All fields are required.';
        return;
    }

    const body = { title, description: desc, deadline: new Date(deadline).toISOString(), priority, status };
    const res = await apiFetch(`${API}/${id}`, { method: 'PUT', body: JSON.stringify(body) });

    if (res?.success) {
        document.getElementById('editModal').classList.add('hidden');
        loadTasks();
    } else {
        errEl.textContent = res?.message || 'Error updating task.';
    }
});

async function markCompleted(id) {
    const res = await apiFetch(`${API}/MarkCompleted/${id}`, { method: 'POST' });
    if (res?.success) loadTasks();
    else alert('Failed to mark task as completed.');
}

loadTasks();