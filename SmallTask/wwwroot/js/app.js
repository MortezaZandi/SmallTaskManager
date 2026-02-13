(function () {
  'use strict';

  const api = {
    projects: {
      list: () => fetch('/api/projects').then(r => r.json()),
      get: id => fetch(`/api/projects/${id}`).then(r => r.ok ? r.json() : null),
      create: (name, description) => fetch('/api/projects', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name, description: description || null }) }).then(r => r.ok ? r.json() : r.json().then(e => Promise.reject(e.message || e))),
      delete: id => fetch(`/api/projects/${id}`, { method: 'DELETE' }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e.message || e)); })
    },
    groups: {
      roots: () => fetch('/api/groups/roots').then(r => r.json()),
      get: id => fetch(`/api/groups/${id}`).then(r => r.ok ? r.json() : null),
      children: id => fetch(`/api/groups/${id}/children`).then(r => r.json()),
      flat: () => fetch('/api/groups/flat').then(r => r.json()),
      withTaskCount: projectId => fetch('/api/groups/with-task-count' + (projectId ? '?projectId=' + projectId : '')).then(r => r.json()),
      create: (name, parentId) => fetch('/api/groups', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name, parentGroupId: parentId || null }) }).then(r => r.ok ? r.json() : r.json().then(e => Promise.reject(e.message || e))),
      rename: (id, name) => fetch(`/api/groups/${id}/rename`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name }) }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e.message || e)); }),
      move: (id, newParentId) => fetch(`/api/groups/${id}/move`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ newParentGroupId: newParentId }) }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e.message || e)); }),
      delete: id => fetch(`/api/groups/${id}`, { method: 'DELETE' }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e.message || e)); })
    },
    users: {
      list: () => fetch('/api/users').then(r => r.json()),
      get: id => fetch(`/api/users/${id}`).then(r => r.ok ? r.json() : null),
      create: (name, iconPath) => fetch('/api/users', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name, iconPath: iconPath || null }) }).then(r => r.ok ? r.json() : r.json().then(e => Promise.reject(e.message || e))),
      update: (id, name, iconPath) => fetch(`/api/users/${id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name, iconPath: iconPath || null }) }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e)); }),
      delete: id => fetch(`/api/users/${id}`, { method: 'DELETE' }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e)); })
    },
    labels: {
      list: () => fetch('/api/labels').then(r => r.json()),
      get: id => fetch(`/api/labels/${id}`).then(r => r.ok ? r.json() : null),
      create: (name, description, color) => fetch('/api/labels', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name, description: description || null, color: color || '#000000' }) }).then(r => r.ok ? r.json() : r.json().then(e => Promise.reject(e.message || e))),
      update: (id, name, description, color) => fetch(`/api/labels/${id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name, description: description || null, color: color || '#000000' }) }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e)); })
    },
    tasks: {
      get: id => fetch(`/api/tasks/${id}`).then(r => r.ok ? r.json() : null),
      filtered: (q) => {
        const params = new URLSearchParams();
        if (q.projectId) params.set('projectId', q.projectId);
        if (q.text) params.set('text', q.text);
        if (q.groupId) params.set('groupId', q.groupId);
        if (q.labelId) params.set('labelId', q.labelId);
        if (q.priority !== undefined && q.priority !== '') params.set('priority', q.priority);
        if (q.status !== undefined && q.status !== '') params.set('status', q.status);
        if (q.assignedUserId) params.set('assignedUserId', q.assignedUserId);
        if (q.taskNumber) params.set('taskNumber', q.taskNumber);
        return fetch('/api/tasks/filtered?' + params).then(r => r.json());
      },
      create: (data) => fetch('/api/tasks', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) }).then(r => r.ok ? r.json() : r.json().then(e => Promise.reject(e.message || e))),
      update: (id, data) => fetch(`/api/tasks/${id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e.message || e)); }),
      delete: id => fetch(`/api/tasks/${id}`, { method: 'DELETE' }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e)); })
    },
    comments: {
      byTask: taskId => fetch(`/api/comments/task/${taskId}`).then(r => r.json()),
      create: (taskId, userId, text) => fetch('/api/comments', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ taskId, userId, text }) }).then(r => r.ok ? r.json() : r.json().then(e => Promise.reject(e.message || e))),
      update: (id, text) => fetch(`/api/comments/${id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ text }) }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e)); }),
      delete: id => fetch(`/api/comments/${id}`, { method: 'DELETE' }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e)); })
    },
    attachments: {
      byTask: taskId => fetch(`/api/attachments/task/${taskId}`).then(r => r.json()),
      upload: (taskId, file) => {
        const fd = new FormData();
        fd.append('taskId', taskId);
        fd.append('file', file);
        return fetch('/api/attachments', { method: 'POST', body: fd }).then(r => r.ok ? r.json() : r.json().then(e => Promise.reject(e.message || e)));
      },
      delete: id => fetch(`/api/attachments/${id}`, { method: 'DELETE' }).then(r => { if (!r.ok) return r.json().then(e => Promise.reject(e)); }),
      downloadUrl: id => `/api/attachments/${id}/download`
    }
  };

  const msg = {
    _modal: null,
    _resolve: null,
    _init() {
      if (this._modal) return;
      const el = document.getElementById('modalMessage');
      if (!el) return;
      this._modal = new bootstrap.Modal(el, { backdrop: 'static' });
      document.getElementById('modalMessageBtnOk')?.addEventListener('click', () => { this._modal.hide(); if (this._resolve) this._resolve(false); });
      document.getElementById('modalMessageBtnCancel')?.addEventListener('click', () => { this._modal.hide(); if (this._resolve) this._resolve(false); });
      document.getElementById('modalMessageBtnYes')?.addEventListener('click', () => { this._modal.hide(); if (this._resolve) this._resolve(true); });
      el.addEventListener('hidden.bs.modal', () => { if (this._resolve) this._resolve(false); this._resolve = null; });
    },
    _show(type, title, text, isQuestion) {
      this._init();
      const header = document.getElementById('modalMessageHeader');
      const titleEl = document.getElementById('modalMessageTitle');
      const iconEl = document.getElementById('modalMessageIcon');
      const textEl = document.getElementById('modalMessageText');
      const okBtn = document.getElementById('modalMessageBtnOk');
      const cancelBtn = document.getElementById('modalMessageBtnCancel');
      const yesBtn = document.getElementById('modalMessageBtnYes');
      header.className = 'modal-header';
      if (type === 'info') { header.classList.add('bg-info', 'bg-opacity-10'); iconEl.className = 'flex-shrink-0 fs-2 text-info'; iconEl.textContent = 'ℹ'; }
      else if (type === 'warning') { header.classList.add('bg-warning', 'bg-opacity-10'); iconEl.className = 'flex-shrink-0 fs-2 text-warning'; iconEl.textContent = '⚠'; }
      else if (type === 'error') { header.classList.add('bg-danger', 'bg-opacity-10'); iconEl.className = 'flex-shrink-0 fs-2 text-danger'; iconEl.textContent = '✕'; }
      else { header.classList.add('bg-primary', 'bg-opacity-10'); iconEl.className = 'flex-shrink-0 fs-2 text-primary'; iconEl.textContent = '?'; }
      titleEl.textContent = title;
      textEl.textContent = text;
      okBtn.style.display = isQuestion ? 'none' : 'inline-block';
      cancelBtn.style.display = isQuestion ? 'inline-block' : 'none';
      yesBtn.style.display = isQuestion ? 'inline-block' : 'none';
      this._modal.show();
    },
    info(text, title = 'Information') { this._show('info', title, text, false); },
    warning(text, title = 'Warning') { this._show('warning', title, text, false); },
    error(text, title = 'Error') { const t = typeof text === 'object' && text?.message ? text.message : String(text || 'An error occurred'); this._show('error', title, t, false); },
    confirm(text, title = 'Confirm') {
      this._init();
      this._show('question', title || 'Confirm', text, true);
      return new Promise(resolve => { this._resolve = resolve; });
    }
  };

  let state = {
    projects: [],
    groups: [],
    users: [],
    labels: [],
    tasks: [],
    selectedGroupId: null,
    selectedTaskId: null,
    currentUserId: parseInt(localStorage.getItem('smallTask_currentUserId') || '0', 10) || null,
    filter: getStoredFilter()
  };

  let taskDescriptionQuill = null;
  function initTaskDescriptionEditor() {
    const el = document.getElementById('modalTaskDescriptionEditor');
    if (!el || taskDescriptionQuill || typeof Quill === 'undefined') return;
    taskDescriptionQuill = new Quill('#modalTaskDescriptionEditor', {
      theme: 'snow',
      placeholder: 'Enter description...',
      modules: {
        toolbar: [
          [{ header: [1, 2, 3, false] }],
          ['bold', 'italic', 'underline', 'strike'],
          [{ color: [] }, { background: [] }],
          [{ align: [] }],
          [{ list: 'ordered' }, { list: 'bullet' }],
          ['link'],
          ['clean']
        ]
      }
    });
  }
  function getTaskDescriptionHtml() {
    if (!taskDescriptionQuill) return null;
    const html = taskDescriptionQuill.root.innerHTML.trim();
    if (!html || html === '<p><br></p>' || html === '<p></p>') return null;
    return html;
  }
  function setTaskDescriptionHtml(html) {
    if (!taskDescriptionQuill) return;
    taskDescriptionQuill.clipboard.dangerouslyPasteHTML(html || '<p><br></p>');
  }
  function renderTaskLabelCheckboxes(selectedLabelIds) {
    const container = document.getElementById('modalTaskLabelsList');
    if (!container) return;
    container.innerHTML = '';
    Array.from(state.labels).forEach(l => {
      const label = document.createElement('label');
      label.className = 'd-flex align-items-center gap-2 mb-1 form-check';
      const cb = document.createElement('input');
      cb.type = 'checkbox';
      cb.className = 'form-check-input';
      cb.value = l.labelId;
      if (selectedLabelIds.indexOf(l.labelId) !== -1) cb.checked = true;
      const span = document.createElement('span');
      span.className = 'form-check-label';
      span.style.color = l.color || '#333';
      span.textContent = l.name;
      label.appendChild(cb);
      label.appendChild(span);
      container.appendChild(label);
    });
    if (state.labels.length === 0) {
      container.innerHTML = '<span class="text-muted small">No labels. Create labels from the Labels menu.</span>';
    }
  }

  function getStoredFilter() {
    try {
      const s = localStorage.getItem('smallTask_filter');
      const f = s ? JSON.parse(s) : {};
      const lastProject = parseInt(localStorage.getItem('smallTask_lastProjectId') || '0', 10);
      if (!f.projectId && lastProject) f.projectId = lastProject;
      return f;
    } catch (_) { return {}; }
  }
  function setStoredFilter(f) {
    localStorage.setItem('smallTask_filter', JSON.stringify(f));
    if (f.projectId) localStorage.setItem('smallTask_lastProjectId', f.projectId);
  }

  function getCurrentFilter() {
    const projectId = document.getElementById('filterProject')?.value;
    const text = document.getElementById('filterText')?.value?.trim() || '';
    const groupId = document.getElementById('filterGroup')?.value;
    const labelId = document.getElementById('filterLabel')?.value;
    const priority = document.getElementById('filterPriority')?.value;
    const status = document.getElementById('filterStatus')?.value;
    const assignedUserId = document.getElementById('filterUser')?.value;
    const taskNumber = document.getElementById('filterTaskNumber')?.value;
    return {
      projectId: projectId ? parseInt(projectId, 10) : undefined,
      text: text || undefined,
      groupId: groupId ? parseInt(groupId, 10) : undefined,
      labelId: labelId ? parseInt(labelId, 10) : undefined,
      priority: priority !== '' ? parseInt(priority, 10) : undefined,
      status: status !== '' ? parseInt(status, 10) : undefined,
      assignedUserId: assignedUserId ? parseInt(assignedUserId, 10) : undefined,
      taskNumber: taskNumber ? parseInt(taskNumber, 10) : undefined
    };
  }
  function setFilterToInputs(f) {
    const el = (id, val) => { const e = document.getElementById(id); if (e) e.value = val != null ? val : ''; };
    el('filterProject', f.projectId);
    el('filterText', f.text);
    el('filterGroup', f.groupId);
    el('filterLabel', f.labelId);
    el('filterPriority', f.priority);
    el('filterStatus', f.status);
    el('filterUser', f.assignedUserId);
    el('filterTaskNumber', f.taskNumber);
  }

  function statusClass(status) {
    if (status === 0) return 'task-card-status-todo';
    if (status === 1) return 'task-card-status-inprogress';
    if (status === 2) return 'task-card-status-done';
    return 'task-card-status-deleted';
    }

  function priorityStyle(p) { return p === 0 ? 'task-card-low' : p === 1 ? 'task-card-medium' : 'task-card-high'; }
  function priorityText(p) { return p === 0 ? 'کم' : p === 1 ? 'متوسط' : 'بالا'; }
  function statusText(s) { return s === 0 ? 'انجام نشده' : s === 1 ? 'درحال انجام' : s === 2 ? 'انجام شده' : 'حذف شده'; }
  function formatCreatedAt(createdAt) {
    const d = new Date(createdAt);
    const dateStr = d.getFullYear() + '-' + String(d.getMonth() + 1).padStart(2, '0') + '-' + String(d.getDate()).padStart(2, '0');
    const now = new Date();
    const diffMs = now - d;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);
    let ago = '';
    if (diffMins < 1) ago = 'هم اکنون';
    else if (diffMins < 60) ago = diffMins + ' دقیقه قبل';
    else if (diffHours < 24) ago = diffHours + ' ساعت قبل';
    else ago = diffDays + ' روز قبل';
    return ago;
  }

  function renderGroupTree(nodes, parentEl, level) {
    if (!nodes || !nodes.length) return;
      level = level || 0;

      if (parentEl.childElementCount == 0) {
          const defaultGroup = {
              groupId: '',
              parentGroupId: null,
              name: "همه",
              isDeleted: false,
              createdAt: new Date()
          };

          nodes.unshift(defaultGroup);
      }

    const ul = document.createElement('div');
    ul.className = level ? 'group-tree-children' : '';
      Array.from(nodes).forEach(g => {
      const div = document.createElement('div');
      div.className = 'group-tree-item' + (state.selectedGroupId === g.groupId ? ' selected' : '');
      div.dataset.groupId = g.groupId;
      div.textContent = g.name;

          //if (g.groupId!='')
          div.draggable = false;

      div.dataset.dragging = 'false';
      div.addEventListener('dragstart', function (e) {
        e.dataTransfer.setData('text/plain', g.groupId);
        e.dataTransfer.effectAllowed = 'move';
        div.dataset.dragging = 'true';
      });
      div.addEventListener('dragend', function () { div.dataset.dragging = 'false'; });
      div.addEventListener('dragover', function (e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
        div.classList.add('bg-primary', 'bg-opacity-25');
      });
      div.addEventListener('dragleave', function () { div.classList.remove('bg-primary', 'bg-opacity-25'); });
      div.addEventListener('drop', function (e) {
        e.preventDefault();
        div.classList.remove('bg-primary', 'bg-opacity-25');
        const srcId = parseInt(e.dataTransfer.getData('text/plain'), 10);
        if (srcId === g.groupId) return;
        api.groups.move(srcId, g.groupId).then(() => loadGroupsTree()).catch(err => msg.error(err));
      });
          div.addEventListener('click', function () {
              state.selectedGroupId = g.groupId;
              document.querySelectorAll('.group-tree-item').forEach(el => el.classList.remove('selected'))
              div.classList.add('selected');
              var _group = document.getElementById('filterGroup');
              _group.value = g.groupId;
              loadTasks();
          });
          div.innerHTML = '<svg aria-hidden="true" height="16" viewBox="0 0 16 16" version="1.1" width="16" data-view-component="true" class="octicon octicon-chevron-right">    <path d="M6.22 3.22a.75.75 0 0 1 1.06 0l4.25 4.25a.75.75 0 0 1 0 1.06l-4.25 4.25a.751.751 0 0 1-1.042-.018.751.751 0 0 1-.018-1.042L9.94 8 6.22 4.28a.75.75 0 0 1 0-1.06Z"></path></svg>  ' + g.name;
      ul.appendChild(div);
      if (g.children && g.children.length) renderGroupTree(g.children, ul, level + 1);
    });
    parentEl.appendChild(ul);
  }

  async function loadGroupsTree() {
    const roots = await api.groups.roots();
    const build = async (items) => {
        const result = [];
        for (const g of Array.from(items)) {
        const children = await api.groups.children(g.groupId);
        result.push({ ...g, children: children.length ? await build(children) : [] });
      }
      return result;
    };
    state.groups = await build(roots);
    const container = document.getElementById('groupTree');
    if (!container) return;
    container.innerHTML = '';
    renderGroupTree(state.groups, container);
  }

  function loadUsers() {
    return api.users.list().then(list => {
      state.users = list;
      const sel = id => document.getElementById(id);
      ['filterUser', 'modalTaskAssignedUser'].forEach(id => {
        const s = sel(id);
        if (!s) return;
        const current = s.value;
        s.innerHTML = '<option value="">— None —</option>';
          Array.from(list).forEach(u => {
          const opt = document.createElement('option');
          opt.value = u.userId;
          opt.textContent = u.name;
          s.appendChild(opt);
        });
        if (current) s.value = current;
      });
      return list;
    });
  }

  function loadProjects() {
    return api.projects.list().then(list => {
      state.projects = list;
      const filterProject = document.getElementById('filterProject');
      if (filterProject) {
        const cur = filterProject.value;
        filterProject.innerHTML = '<option value="">All Projects</option>';
        list.forEach(p => {
          const opt = document.createElement('option');
          opt.value = p.projectId;
          opt.textContent = p.name + (p.taskCount != null ? ' (' + p.taskCount + ')' : '');
          filterProject.appendChild(opt);
        });
        if (cur) filterProject.value = cur;
        else if (list.length && state.filter.projectId) filterProject.value = state.filter.projectId;
        else if (list.length && parseInt(localStorage.getItem('smallTask_lastProjectId') || '0', 10)) filterProject.value = localStorage.getItem('smallTask_lastProjectId');
      }
      return list;
    });
  }

  function loadLabels() {
    return api.labels.list().then(list => {
      state.labels = list;
      const filterLabel = document.getElementById('filterLabel');
      if (filterLabel) {
        const cur = filterLabel.value;
        filterLabel.innerHTML = '<option value="">All Labels</option>';
          Array.from(list).forEach(l => {
          const opt = document.createElement('option');
          opt.value = l.labelId;
          opt.textContent = l.name;
          filterLabel.appendChild(opt);
        });
        if (cur) filterLabel.value = cur;
      }
      return list;
    });
  }

  function loadGroupOptions(selectId, excludeGroupId) {
    return api.groups.flat().then(flat => {
      const s = document.getElementById(selectId);
      if (!s) return;
      const cur = s.value;
      s.innerHTML = '<option value="">— None —</option>';
      flat.filter(g => g.groupId !== excludeGroupId).forEach(g => {
        const opt = document.createElement('option');
        opt.value = g.groupId;
        opt.textContent = g.name;
        s.appendChild(opt);
      });
      if (cur) s.value = cur;
    });
  }

  function loadTasks() {
    const q = getCurrentFilter();
    return api.tasks.filtered(q).then(list => {
      state.tasks = list;
      const columns = { 0: document.getElementById('columnTodo'), 1: document.getElementById('columnInProgress'), 2: document.getElementById('columnDone') };
      [0, 1, 2].forEach(status => {
        const col = columns[status];
        if (!col) return;
          col.innerHTML = '';
         Array.from(list).filter(t => t.status === status).forEach(task => {
             const card = document.createElement('div');
             card.className = 'task-card ' + priorityStyle(task.priority);
          card.dataset.taskId = task.taskId;
          card.innerHTML = `
            <div class="task-card-title" style="text-align:right">${escapeHtml(task.title)}</div>
            <div class="task-card-meta">
              <div class="task-card-labels">#${task.taskNumber} ${(task.taskLabels || []).map(tl => tl.label ? `<span class="task-card-label" style="background:${tl.label.color}20;color:${tl.label.color}">${escapeHtml(tl.label.name)}</span>` : '').join('')}</div>
              <span class="task-card-priority">${priorityText(task.priority)}</span>
              ${task.assignedUser ? `<img class="task-card-assigned" src="${escapeHtml(task.assignedUser.iconPath || '/favicon.ico')}" alt="" title="${escapeHtml(task.assignedUser.name)}" onerror="this.src='/favicon.ico'" />` : ''}
            </div>`;
          card.addEventListener('click', function (e) {
            if (e.target.closest('.task-card-status-menu-trigger')) return;
            state.selectedTaskId = task.taskId;
            openTaskDetailsModal(task.taskId);
          });
          card.addEventListener('contextmenu', function (e) {
            e.preventDefault();
            state.selectedTaskId = task.taskId;
            const menu = document.getElementById('statusMenuContainer');
            if (menu) {
              menu.style.display = 'block';
              menu.style.position = 'fixed';
              menu.style.left = e.clientX + 'px';
              menu.style.top = e.clientY + 'px';
            }
          });
          col.appendChild(card);
         });
          document.getElementById('taskCount' + status).innerHTML = Array.from(list).filter(t => t.status === status).length;
      });
    });
  }

  function escapeHtml(s) {
    if (!s) return '';
    const d = document.createElement('div');
    d.textContent = s;
    return d.innerHTML;
  }

  function openTaskDetailsModal(taskId) {
    if (!taskId) return;
    api.tasks.get(taskId).then(task => {
      if (!task) return;
      document.getElementById('modalTaskDetailsId').value = task.taskId;
      document.getElementById('modalTaskDetailsHeader').textContent = 'Task #' + task.taskNumber + ' – Details';
      document.getElementById('modalTaskDetailsTitle').textContent = task.title || '';
      const createdAtEl = document.getElementById('modalTaskDetailsCreatedAt');
      if (task.createdAt) {
        createdAtEl.textContent = formatCreatedAt(task.createdAt);
      } else {
        createdAtEl.textContent = '—';
      }
      const descEl = document.getElementById('modalTaskDetailsDescription');
      if (task.description && task.description.trim()) {
        descEl.innerHTML = task.description;
      } else {
        descEl.textContent = '—';
      }
      document.getElementById('modalTaskDetailsStatus').textContent = statusText(task.status);
      document.getElementById('modalTaskDetailsPriority').textContent = priorityText(task.priority);
      document.getElementById('modalTaskDetailsAssignedUser').innerHTML = task.assignedUser
        ? `<img src="${escapeHtml(task.assignedUser.iconPath || '/favicon.ico')}" alt="" class="me-1" style="width:20px;height:20px;border-radius:50%;" onerror="this.src='/favicon.ico'" /> ${escapeHtml(task.assignedUser.name)}`
        : '—';
      document.getElementById('modalTaskDetailsProject').textContent = (task.project && task.project.name) ? task.project.name : '—';
      document.getElementById('modalTaskDetailsGroup').textContent = (task.group && task.group.name) ? task.group.name : '—';
      const labelsHtml = (task.taskLabels || []).filter(tl => tl.label).map(tl =>
        `<span class="task-card-label me-1" style="background:${tl.label.color}20;color:${tl.label.color}">${escapeHtml(tl.label.name)}</span>`
      ).join('') || '—';
      document.getElementById('modalTaskDetailsLabels').innerHTML = labelsHtml;
      api.comments.byTask(taskId).then(comments => {
        const list = document.getElementById('modalTaskDetailsCommentsList');
        list.innerHTML = comments.length
          ? comments.map(c => `<div class="comment-item small mb-2"><div class="text-muted">${escapeHtml((c.user && c.user.name) || '')} · ${new Date(c.createdAt).toLocaleString()}</div><div>${escapeHtml(c.text)}</div></div>`).join('')
          : '<div class="text-muted small">No comments.</div>';
      });
      api.attachments.byTask(taskId).then(attachments => {
        const list = document.getElementById('modalTaskDetailsAttachmentsList');
        list.innerHTML = attachments.length
          ? attachments.map(a => `<div class="attachment-item small mb-1"><a href="${api.attachments.downloadUrl(a.attachmentId)}" target="_blank" download="${escapeHtml(a.originalFileName)}">${escapeHtml(a.originalFileName)}</a></div>`).join('')
          : '<div class="text-muted small">No attachments.</div>';
      });
      new bootstrap.Modal(document.getElementById('modalTaskDetails')).show();
    });
  }

  function openTaskModal(taskId) {
    if (!taskId) {
      document.getElementById('modalTaskHeader').textContent = 'Create Task';
      document.getElementById('modalTaskId').value = '';
      document.getElementById('modalTaskProjectId').value = document.getElementById('filterProject')?.value || (state.projects[0]?.projectId || '');
      document.getElementById('modalTaskTitle').value = '';
      setTaskDescriptionHtml('');
      document.getElementById('modalTaskPriority').value = '1';
      document.getElementById('modalTaskStatus').value = '0';
      document.getElementById('modalTaskAssignedUser').value = '';
      document.getElementById('modalTaskGroup').value = '';
      renderTaskLabelCheckboxes([]);
      document.getElementById('modalTaskCommentsList').innerHTML = '';
      document.getElementById('modalTaskAttachmentsList').innerHTML = '';
      new bootstrap.Modal(document.getElementById('modalTask')).show();
      loadGroupOptions('modalTaskGroup');
      return;
    }
    api.tasks.get(taskId).then(task => {
      if (!task) return;
      document.getElementById('modalTaskHeader').textContent = 'Edit Task';
      document.getElementById('modalTaskId').value = task.taskId;
      document.getElementById('modalTaskProjectId').value = task.projectId != null ? String(task.projectId) : '';
      document.getElementById('modalTaskTitle').value = task.title;
      setTaskDescriptionHtml(task.description || '');
      document.getElementById('modalTaskPriority').value = String(task.priority);
      document.getElementById('modalTaskStatus').value = String(task.status);
      document.getElementById('modalTaskAssignedUser').value = task.assignedUserId != null ? String(task.assignedUserId) : '';
      document.getElementById('modalTaskGroup').value = task.groupId != null ? String(task.groupId) : '';
      const selectedLabelIds = (task.taskLabels || []).map(tl => tl.labelId);
      renderTaskLabelCheckboxes(selectedLabelIds);
      loadGroupOptions('modalTaskGroup');
      api.comments.byTask(taskId).then(comments => {
        const list = document.getElementById('modalTaskCommentsList');
        list.innerHTML = '';
          Array.from(comments).forEach(c => {
          const div = document.createElement('div');
          div.className = 'comment-item';
          div.innerHTML = `<div class="comment-item-header">${escapeHtml((c.user && c.user.name) || '')} · ${new Date(c.createdAt).toLocaleString()}</div><div>${escapeHtml(c.text)}</div>`;
          list.appendChild(div);
        });
      });
      api.attachments.byTask(taskId).then(attachments => {
        const list = document.getElementById('modalTaskAttachmentsList');
        list.innerHTML = '';
          Array.from(attachments).forEach(a => {
          const div = document.createElement('div');
          div.className = 'attachment-item';
          div.innerHTML = `<a href="${api.attachments.downloadUrl(a.attachmentId)}" target="_blank" download="${escapeHtml(a.originalFileName)}">${escapeHtml(a.originalFileName)}</a> <button type="button" class="btn btn-sm btn-outline-danger delete-attachment" data-id="${a.attachmentId}">Delete</button>`;
          list.appendChild(div);
        });
        list.querySelectorAll('.delete-attachment').forEach(btn => {
          btn.addEventListener('click', function () {
            msg.confirm('Delete this attachment?').then(confirmed => { if (!confirmed) return; api.attachments.delete(parseInt(this.dataset.id, 10)).then(() => openTaskModal(taskId)).catch(e => msg.error(e)); });
          });
        });
      });
      new bootstrap.Modal(document.getElementById('modalTask')).show();
    });
  }

  function saveTaskModal() {
    const id = document.getElementById('modalTaskId').value;
    const titleEl = document.getElementById('modalTaskTitle');
    const title = titleEl ? titleEl.value.trim() : '';
    if (!title) { msg.warning('Title is required.'); return; }
    const description = getTaskDescriptionHtml();
    const priority = parseInt(document.getElementById('modalTaskPriority').value, 10);
    const status = parseInt(document.getElementById('modalTaskStatus').value, 10);
    const assignedUserId = document.getElementById('modalTaskAssignedUser').value ? parseInt(document.getElementById('modalTaskAssignedUser').value, 10) : null;
    const groupId = document.getElementById('modalTaskGroup').value ? parseInt(document.getElementById('modalTaskGroup').value, 10) : null;
    const labelIds = Array.from(document.querySelectorAll('#modalTaskLabelsList input[type="checkbox"]:checked')).map(cb => parseInt(cb.value, 10));
    const projectId = parseInt(document.getElementById('modalTaskProjectId')?.value || document.getElementById('filterProject')?.value || '0', 10) || (state.projects[0] && state.projects[0].projectId);
    if (!projectId) { msg.warning('Select a project first.'); return; }
    const data = { projectId, title, description, status, priority, assignedUserId, groupId, labelIds };
    if (!id) {
      api.tasks.create(data).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalTask')).hide(); loadTasks(); }).catch(e => msg.error(e));
    } else {
      api.tasks.update(parseInt(id, 10), data).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalTask')).hide(); loadTasks(); }).catch(e => msg.error(e));
    }
  }

  document.getElementById('modalTaskAddComment')?.addEventListener('click', function () {
    const taskId = document.getElementById('modalTaskId').value;
    if (!taskId) { msg.warning('Save the task first before adding comments.'); return; }
    if (!state.currentUserId) { msg.warning('Select a user (or create one) to post comments.'); return; }
    document.getElementById('modalCommentId').value = '';
    document.getElementById('modalCommentTaskId').value = taskId;
    document.getElementById('modalCommentText').value = '';
    document.getElementById('modalCommentTitle').textContent = 'Add Comment';
    new bootstrap.Modal(document.getElementById('modalComment')).show();
  });

  document.getElementById('modalCommentSave')?.addEventListener('click', function () {
    const id = document.getElementById('modalCommentId').value;
    const taskId = parseInt(document.getElementById('modalCommentTaskId').value, 10);
    const text = document.getElementById('modalCommentText').value.trim();
    if (!text) { msg.warning('Text is required.'); return; }
    if (id) {
      api.comments.update(parseInt(id, 10), text).then(() => {
        bootstrap.Modal.getInstance(document.getElementById('modalComment')).hide();
        openTaskModal(taskId);
      }).catch(e => msg.error(e));
    } else {
      if (!state.currentUserId) { msg.warning('Select a user to post comments.'); return; }
      api.comments.create(taskId, state.currentUserId, text).then(() => {
        bootstrap.Modal.getInstance(document.getElementById('modalComment')).hide();
        openTaskModal(taskId);
      }).catch(e => msg.error(e));
    }
  });

  document.getElementById('modalTaskUploadAttachment')?.addEventListener('click', function () {
    const taskId = document.getElementById('modalTaskId').value;
    if (!taskId) { msg.warning('Save the task first.'); return; }
    document.getElementById('modalTaskFileInput').click();
  });
  document.getElementById('modalTaskFileInput')?.addEventListener('change', function () {
    const taskId = document.getElementById('modalTaskId').value;
    const file = this.files[0];
    if (!file || !taskId) return;
    api.attachments.upload(parseInt(taskId, 10), file).then(() => {
      this.value = '';
      openTaskModal(parseInt(taskId, 10));
    }).catch(e => msg.error(e));
  });

  document.getElementById('modalProjectSave')?.addEventListener('click', function () {
    const id = document.getElementById('modalProjectId').value;
    const name = document.getElementById('modalProjectName').value.trim();
    if (!name) { msg.warning('Name is required.'); return; }
    const description = document.getElementById('modalProjectDescription').value.trim() || null;
    api.projects.create(name, description).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalProject')).hide(); loadProjects(); loadTasks(); }).catch(e => msg.error(e));
  });

  document.getElementById('modalGroupSave')?.addEventListener('click', function () {
    const id = document.getElementById('modalGroupId').value;
    const name = document.getElementById('modalGroupName').value.trim();
    if (!name) { msg.warning('Name is required.'); return; }
    const parentId = document.getElementById('modalGroupParent').value ? parseInt(document.getElementById('modalGroupParent').value, 10) : null;
    if (!id) {
      api.groups.create(name, parentId).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalGroup')).hide(); loadGroupsTree(); loadGroupOptions('filterGroup'); loadGroupOptions('modalTaskGroup'); }).catch(e => msg.error(e));
    } else {
      api.groups.rename(parseInt(id, 10), name).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalGroup')).hide(); loadGroupsTree(); loadGroupOptions('filterGroup'); loadGroupOptions('modalTaskGroup'); }).catch(e => msg.error(e));
    }
  });

  document.getElementById('modalUserSave')?.addEventListener('click', function () {
    const id = document.getElementById('modalUserId').value;
    const name = document.getElementById('modalUserName').value.trim();
    if (!name) { msg.warning('Name is required.'); return; }
    const iconPath = document.getElementById('modalUserIconPath').value.trim() || null;
    if (!id) {
      api.users.create(name, iconPath).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalUser')).hide(); loadUsers(); }).catch(e => msg.error(e));
    } else {
      api.users.update(parseInt(id, 10), name, iconPath).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalUser')).hide(); loadUsers(); }).catch(e => msg.error(e));
    }
  });

  document.getElementById('modalTaskSave')?.addEventListener('click', saveTaskModal);

  document.getElementById('modalTaskDetailsEdit')?.addEventListener('click', function () {
    const taskId = document.getElementById('modalTaskDetailsId').value;
    if (!taskId) return;
    bootstrap.Modal.getInstance(document.getElementById('modalTaskDetails')).hide();
    openTaskModal(parseInt(taskId, 10));
  });
  document.getElementById('modalTaskDetailsDelete')?.addEventListener('click', function () {
    const taskId = document.getElementById('modalTaskDetailsId').value;
    if (!taskId) return;
    msg.confirm('Delete this task?').then(confirmed => { if (!confirmed) return; api.tasks.delete(parseInt(taskId, 10)).then(() => {
      bootstrap.Modal.getInstance(document.getElementById('modalTaskDetails')).hide();
      state.selectedTaskId = null;
      loadTasks();
    }).catch(e => msg.error(e)); });
  });

  document.getElementById('modalLabelSave')?.addEventListener('click', function () {
    const id = document.getElementById('modalLabelId').value;
    const name = document.getElementById('modalLabelName').value.trim();
    if (!name) { msg.warning('Name is required.'); return; }
    const description = document.getElementById('modalLabelDescription').value.trim() || null;
    const color = document.getElementById('modalLabelColor').value || '#000000';
    if (!id) {
      api.labels.create(name, description, color).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalLabel')).hide(); loadLabels(); }).catch(e => msg.error(e));
    } else {
      api.labels.update(parseInt(id, 10), name, description, color).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalLabel')).hide(); loadLabels(); }).catch(e => msg.error(e));
    }
  });

  document.getElementById('menuGroupCreate')?.addEventListener('click', function (e) { e.preventDefault(); document.getElementById('modalGroupTitle').textContent = 'Create Group'; document.getElementById('modalGroupId').value = ''; document.getElementById('modalGroupName').value = ''; document.getElementById('modalGroupParentWrap').style.display = 'block'; loadGroupOptions('modalGroupParent'); new bootstrap.Modal(document.getElementById('modalGroup')).show(); });
  document.getElementById('menuGroupRename')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.selectedGroupId) { msg.warning('Select a group first.'); return; } document.getElementById('modalGroupTitle').textContent = 'Rename Group'; api.groups.get(state.selectedGroupId).then(g => { if (!g) return; document.getElementById('modalGroupId').value = g.groupId; document.getElementById('modalGroupName').value = g.name; document.getElementById('modalGroupParentWrap').style.display = 'none'; new bootstrap.Modal(document.getElementById('modalGroup')).show(); }); });
  document.getElementById('menuGroupList')?.addEventListener('click', function (e) { e.preventDefault(); const projectId = document.getElementById('filterProject')?.value ? parseInt(document.getElementById('filterProject').value, 10) : null; api.groups.withTaskCount(projectId).then(groups => { const el = document.getElementById('listGroupsContent'); el.innerHTML = groups.map(g => `<div class="d-flex align-items-center justify-content-between py-2 border-bottom"><span>${escapeHtml(g.name)} (${g.taskCount != null ? g.taskCount : 0} tasks)</span><button class="btn btn-sm btn-outline-danger delete-group-btn" data-id="${g.groupId}" ${(g.taskCountTotal || 0) > 0 ? 'disabled title="Remove tasks first"' : ''}>Delete</button></div>`).join('') || '<div class="text-muted small">No groups.</div>'; el.querySelectorAll('.delete-group-btn').forEach(btn => { if (!btn.disabled) btn.addEventListener('click', function () { msg.confirm('Delete this group?').then(confirmed => { if (!confirmed) return; api.groups.delete(parseInt(this.dataset.id, 10)).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalListGroups')).hide(); loadGroupsTree(); loadGroupOptions('filterGroup'); loadGroupOptions('modalTaskGroup'); loadTasks(); }).catch(err => msg.error(err)); }); }); }); new bootstrap.Modal(document.getElementById('modalListGroups')).show(); }); });

  document.getElementById('menuTaskCreate')?.addEventListener('click', function (e) { e.preventDefault(); openTaskModal(null); });
  document.getElementById('menuTaskEdit')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.selectedTaskId) { msg.warning('Select a task first (click on a card).'); return; } openTaskModal(state.selectedTaskId); });
  document.getElementById('menuTaskDelete')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.selectedTaskId) { msg.warning('Select a task first.'); return; } msg.confirm('Delete this task?').then(confirmed => { if (!confirmed) return; api.tasks.delete(state.selectedTaskId).then(() => { state.selectedTaskId = null; loadTasks(); }).catch(e => msg.error(e)); }); });

  document.getElementById('menuUserCreate')?.addEventListener('click', function (e) { e.preventDefault(); document.getElementById('modalUserId').value = ''; document.getElementById('modalUserName').value = ''; document.getElementById('modalUserIconPath').value = ''; document.getElementById('modalUserTitle').textContent = 'Create User'; new bootstrap.Modal(document.getElementById('modalUser')).show(); });
    document.getElementById('menuUserEdit')?.addEventListener('click', function (e) { e.preventDefault(); const id = state.currentUserId || (state.users[0] && state.users[0].userId); if (!id) { msg.warning('Select or create a user first.'); return; } api.users.get(id).then(u => { if (!u) return; document.getElementById('modalUserId').value = u.userId; document.getElementById('modalUserName').value = u.name; document.getElementById('modalUserIconPath').value = u.iconPath || ''; document.getElementById('modalUserTitle').textContent = 'Edit User'; new bootstrap.Modal(document.getElementById('modalUser')).show(); }); });
    document.getElementById('menuUserList')?.addEventListener('click', function (e) { e.preventDefault(); api.users.list().then(users => { const el = document.getElementById('listUsersContent'); el.innerHTML = Array.from(users).map(u => `<div class="d-flex align-items-center gap-2 py-2 border-bottom"><span>${escapeHtml(u.name)}</span><button class="btn btn-sm btn-outline-primary ms-auto" data-user-id="${u.userId}">Use as comment author</button></div>`).join(''); el.querySelectorAll('[data-user-id]').forEach(btn => btn.addEventListener('click', function () { state.currentUserId = parseInt(this.dataset.userId, 10); localStorage.setItem('smallTask_currentUserId', state.currentUserId); bootstrap.Modal.getInstance(document.getElementById('modalListUsers')).hide(); })); new bootstrap.Modal(document.getElementById('modalListUsers')).show(); }); });

  document.getElementById('menuProjectCreate')?.addEventListener('click', function (e) { e.preventDefault(); document.getElementById('modalProjectId').value = ''; document.getElementById('modalProjectName').value = ''; document.getElementById('modalProjectDescription').value = ''; document.getElementById('modalProjectTitle').textContent = 'Create Project'; new bootstrap.Modal(document.getElementById('modalProject')).show(); });
  document.getElementById('menuProjectList')?.addEventListener('click', function (e) { e.preventDefault(); api.projects.list().then(projects => { const el = document.getElementById('listProjectsContent'); el.innerHTML = projects.map(p => `<div class="d-flex flex-column py-2 border-bottom"><div class="fw-bold">${escapeHtml(p.name)}</div><div class="text-muted small">${escapeHtml(p.description || '')}</div><div class="small">${p.taskCount != null ? p.taskCount + ' tasks' : ''}</div></div>`).join('') || '<div class="text-muted small">No projects.</div>'; new bootstrap.Modal(document.getElementById('modalListProjects')).show(); }); });
  document.getElementById('menuProjectDelete')?.addEventListener('click', function (e) { e.preventDefault(); api.projects.list().then(projects => { const el = document.getElementById('deleteProjectList'); el.innerHTML = projects.map(p => `<div class="d-flex align-items-center justify-content-between py-2 border-bottom"><span>${escapeHtml(p.name)} (${p.taskCount != null ? p.taskCount : 0} tasks)</span><button class="btn btn-sm btn-outline-danger delete-project-btn" data-id="${p.projectId}" ${(p.taskCount || 0) > 0 ? 'disabled title="Remove tasks first"' : ''}>Delete</button></div>`).join('') || '<div class="text-muted small">No projects.</div>'; el.querySelectorAll('.delete-project-btn').forEach(btn => { if (!btn.disabled) btn.addEventListener('click', function () { msg.confirm('Delete this project?').then(confirmed => { if (!confirmed) return; api.projects.delete(parseInt(this.dataset.id, 10)).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalDeleteProject')).hide(); loadProjects(); loadTasks(); }).catch(err => msg.error(err)); }); }); }); new bootstrap.Modal(document.getElementById('modalDeleteProject')).show(); }); });

  document.getElementById('menuLabelCreate')?.addEventListener('click', function (e) { e.preventDefault(); document.getElementById('modalLabelId').value = ''; document.getElementById('modalLabelName').value = ''; document.getElementById('modalLabelDescription').value = ''; document.getElementById('modalLabelColor').value = '#000000'; document.getElementById('modalLabelTitle').textContent = 'Create Label'; new bootstrap.Modal(document.getElementById('modalLabel')).show(); });
  document.getElementById('menuLabelEdit')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.labels.length) { msg.warning('Create a label first.'); return; } const l = state.labels[0]; document.getElementById('modalLabelId').value = l.labelId; document.getElementById('modalLabelName').value = l.name; document.getElementById('modalLabelDescription').value = l.description || ''; document.getElementById('modalLabelColor').value = l.color || '#000000'; document.getElementById('modalLabelTitle').textContent = 'Edit Label'; new bootstrap.Modal(document.getElementById('modalLabel')).show(); });
  document.getElementById('menuLabelList')?.addEventListener('click', function (e) { e.preventDefault(); api.labels.list().then(labels => { const el = document.getElementById('listLabelsContent'); el.innerHTML = labels.map(l => `<div class="d-flex align-items-center gap-2 py-2 border-bottom"><span class="task-card-label" style="background:${l.color}20;color:${l.color}">${escapeHtml(l.name)}</span><span class="text-muted small">${escapeHtml(l.description || '')}</span></div>`).join(''); new bootstrap.Modal(document.getElementById('modalListLabels')).show(); }); });

  document.getElementById('btnApplyFilter')?.addEventListener('click', function () { setStoredFilter(getCurrentFilter()); loadTasks(); });
  document.querySelectorAll('#filterArea input, #filterArea select').forEach(el => el.addEventListener('change', function () { setStoredFilter(getCurrentFilter()); loadTasks(); }));
  document.querySelectorAll('#filterMenu [data-filter="default"]').forEach(el => el.addEventListener('click', function (e) { e.preventDefault(); setFilterToInputs({}); setStoredFilter({}); loadTasks(); }));
  document.querySelectorAll('#filterMenu [data-filter="save"]').forEach(el => el.addEventListener('click', function (e) { e.preventDefault(); setStoredFilter(getCurrentFilter()); msg.info('Filter saved.'); }));
  document.querySelectorAll('#filterMenu [data-filter="delete"]').forEach(el => el.addEventListener('click', function (e) { e.preventDefault(); setStoredFilter({}); setFilterToInputs({}); loadTasks(); }));

  document.querySelectorAll('#statusMenuContainer [data-status]').forEach(el => el.addEventListener('click', function (e) { e.preventDefault(); const status = this.dataset.status; const taskId = state.selectedTaskId; document.getElementById('statusMenuContainer').style.display = 'none'; if (!taskId) return; if (status === 'delete') { msg.confirm('Delete this task?').then(confirmed => { if (!confirmed) return; api.tasks.delete(taskId).then(() => loadTasks()).catch(e => msg.error(e)); }); return; } api.tasks.get(taskId).then(t => { if (!t) return; api.tasks.update(taskId, { title: t.title, description: t.description, status: parseInt(status, 10), priority: t.priority, assignedUserId: t.assignedUserId, groupId: t.groupId, labelIds: (t.taskLabels || []).map(tl => tl.labelId) }).then(() => loadTasks()).catch(e => msg.error(e)); }); }));

  document.addEventListener('click', function () { document.getElementById('statusMenuContainer').style.display = 'none'; });

  initTaskDescriptionEditor();
  loadProjects().then(() => {
    setFilterToInputs(state.filter);
    loadGroupsTree().then(() => { loadGroupOptions('filterGroup'); loadGroupOptions('modalTaskGroup'); });
    loadUsers();
    loadLabels().then(() => loadTasks());
  });
})();
