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

loadTasks();