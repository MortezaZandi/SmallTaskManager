(function () {
  'use strict';

  const api = {
    groups: {
      roots: () => fetch('/api/groups/roots').then(r => r.json()),
      get: id => fetch(`/api/groups/${id}`).then(r => r.ok ? r.json() : null),
      children: id => fetch(`/api/groups/${id}/children`).then(r => r.json()),
      flat: () => fetch('/api/groups/flat').then(r => r.json()),
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

  let state = {
    groups: [],
    users: [],
    labels: [],
    tasks: [],
    selectedGroupId: null,
    selectedTaskId: null,
    currentUserId: parseInt(localStorage.getItem('smallTask_currentUserId') || '0', 10) || null,
    filter: getStoredFilter()
  };

  function getStoredFilter() {
    try {
      const s = localStorage.getItem('smallTask_filter');
      return s ? JSON.parse(s) : {};
    } catch (_) { return {}; }
  }
  function setStoredFilter(f) {
    localStorage.setItem('smallTask_filter', JSON.stringify(f));
  }

  function getCurrentFilter() {
    const text = document.getElementById('filterText')?.value?.trim() || '';
    const groupId = document.getElementById('filterGroup')?.value;
    const labelId = document.getElementById('filterLabel')?.value;
    const priority = document.getElementById('filterPriority')?.value;
    const status = document.getElementById('filterStatus')?.value;
    const assignedUserId = document.getElementById('filterUser')?.value;
    const taskNumber = document.getElementById('filterTaskNumber')?.value;
    return {
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
  function priorityText(p) { return p === 0 ? 'Low' : p === 1 ? 'Medium' : 'High'; }

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
        api.groups.move(srcId, g.groupId).then(() => loadGroupsTree()).catch(err => alert(err));
      });
          div.addEventListener('click', function () {
              state.selectedGroupId = g.groupId;
              document.querySelectorAll('.group-tree-item').forEach(el => el.classList.remove('selected'))
              div.classList.add('selected');
              var _group = document.getElementById('filterGroup');
              _group.value = g.groupId;
              loadTasks();
          });

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
          card.className = 'task-card ' + statusClass(task.status);
          card.dataset.taskId = task.taskId;
          card.innerHTML = `
            <div class="task-card-number">#${task.taskNumber}</div>
            <div class="task-card-title">${escapeHtml(task.title)}</div>
            <div class="task-card-meta">
              <div class="task-card-labels">${(task.taskLabels || []).map(tl => tl.label ? `<span class="task-card-label" style="background:${tl.label.color}20;color:${tl.label.color}">${escapeHtml(tl.label.name)}</span>` : '').join('')}</div>
              <span class="task-card-priority">${priorityText(task.priority)}</span>
              ${task.assignedUser ? `<img class="task-card-assigned" src="${escapeHtml(task.assignedUser.iconPath || '/favicon.ico')}" alt="" title="${escapeHtml(task.assignedUser.name)}" onerror="this.src='/favicon.ico'" />` : ''}
            </div>`;
          card.addEventListener('click', function (e) {
            if (e.target.closest('.task-card-status-menu-trigger')) return;
            state.selectedTaskId = task.taskId;
            openTaskModal(task.taskId);
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
      });
    });
  }

  function escapeHtml(s) {
    if (!s) return '';
    const d = document.createElement('div');
    d.textContent = s;
    return d.innerHTML;
  }

  function openTaskModal(taskId) {
    if (!taskId) {
      document.getElementById('modalTaskHeader').textContent = 'Create Task';
      document.getElementById('modalTaskId').value = '';
      document.getElementById('modalTaskTitle').value = '';
      document.getElementById('modalTaskDescription').value = '';
      document.getElementById('modalTaskPriority').value = '1';
      document.getElementById('modalTaskStatus').value = '0';
      document.getElementById('modalTaskAssignedUser').value = '';
      document.getElementById('modalTaskGroup').value = '';
        document.getElementById('modalTaskLabels').innerHTML = '';
        Array.from(state.labels).forEach(l => {
        const opt = document.createElement('option');
        opt.value = l.labelId;
        opt.textContent = l.name;
        document.getElementById('modalTaskLabels').appendChild(opt);
      });
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
      document.getElementById('modalTaskTitle').value = task.title;
      document.getElementById('modalTaskDescription').value = task.description || '';
      document.getElementById('modalTaskPriority').value = String(task.priority);
      document.getElementById('modalTaskStatus').value = String(task.status);
      document.getElementById('modalTaskAssignedUser').value = task.assignedUserId != null ? String(task.assignedUserId) : '';
      document.getElementById('modalTaskGroup').value = task.groupId != null ? String(task.groupId) : '';
      const labelsEl = document.getElementById('modalTaskLabels');
        labelsEl.innerHTML = '';
        Array.from(state.labels).forEach(l => {
        const opt = document.createElement('option');
        opt.value = l.labelId;
        opt.textContent = l.name;
        opt.selected = (task.taskLabels || []).some(tl => tl.labelId === l.labelId);
        labelsEl.appendChild(opt);
      });
      loadGroupOptions('modalTaskGroup', task.groupId);
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
            if (!confirm('Delete this attachment?')) return;
            api.attachments.delete(parseInt(this.dataset.id, 10)).then(() => openTaskModal(taskId));
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
    if (!title) { alert('Title is required.'); return; }
    const description = document.getElementById('modalTaskDescription').value.trim() || null;
    const priority = parseInt(document.getElementById('modalTaskPriority').value, 10);
    const status = parseInt(document.getElementById('modalTaskStatus').value, 10);
    const assignedUserId = document.getElementById('modalTaskAssignedUser').value ? parseInt(document.getElementById('modalTaskAssignedUser').value, 10) : null;
    const groupId = document.getElementById('modalTaskGroup').value ? parseInt(document.getElementById('modalTaskGroup').value, 10) : null;
    const labelOpts = document.getElementById('modalTaskLabels').selectedOptions;
    const labelIds = Array.from(labelOpts).map(o => parseInt(o.value, 10));
    const data = { title, description, status, priority, assignedUserId, groupId, labelIds };
    if (!id) {
      api.tasks.create(data).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalTask')).hide(); loadTasks(); }).catch(e => alert(e));
    } else {
      api.tasks.update(parseInt(id, 10), data).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalTask')).hide(); loadTasks(); }).catch(e => alert(e));
    }
  }

  document.getElementById('modalTaskAddComment')?.addEventListener('click', function () {
    const taskId = document.getElementById('modalTaskId').value;
    if (!taskId) { alert('Save the task first before adding comments.'); return; }
    if (!state.currentUserId) { alert('Select a user (or create one) to post comments.'); return; }
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
    if (!text) { alert('Text is required.'); return; }
    if (id) {
      api.comments.update(parseInt(id, 10), text).then(() => {
        bootstrap.Modal.getInstance(document.getElementById('modalComment')).hide();
        openTaskModal(taskId);
      }).catch(e => alert(e));
    } else {
      if (!state.currentUserId) { alert('Select a user to post comments.'); return; }
      api.comments.create(taskId, state.currentUserId, text).then(() => {
        bootstrap.Modal.getInstance(document.getElementById('modalComment')).hide();
        openTaskModal(taskId);
      }).catch(e => alert(e));
    }
  });

  document.getElementById('modalTaskUploadAttachment')?.addEventListener('click', function () {
    const taskId = document.getElementById('modalTaskId').value;
    if (!taskId) { alert('Save the task first.'); return; }
    document.getElementById('modalTaskFileInput').click();
  });
  document.getElementById('modalTaskFileInput')?.addEventListener('change', function () {
    const taskId = document.getElementById('modalTaskId').value;
    const file = this.files[0];
    if (!file || !taskId) return;
    api.attachments.upload(parseInt(taskId, 10), file).then(() => {
      this.value = '';
      openTaskModal(parseInt(taskId, 10));
    }).catch(e => alert(e));
  });

  document.getElementById('modalGroupSave')?.addEventListener('click', function () {
    const id = document.getElementById('modalGroupId').value;
    const name = document.getElementById('modalGroupName').value.trim();
    if (!name) { alert('Name is required.'); return; }
    const parentId = document.getElementById('modalGroupParent').value ? parseInt(document.getElementById('modalGroupParent').value, 10) : null;
    if (!id) {
      api.groups.create(name, parentId).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalGroup')).hide(); loadGroupsTree(); loadGroupOptions('filterGroup'); loadGroupOptions('modalTaskGroup'); }).catch(e => alert(e));
    } else {
      api.groups.rename(parseInt(id, 10), name).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalGroup')).hide(); loadGroupsTree(); loadGroupOptions('filterGroup'); loadGroupOptions('modalTaskGroup'); }).catch(e => alert(e));
    }
  });

  document.getElementById('modalUserSave')?.addEventListener('click', function () {
    const id = document.getElementById('modalUserId').value;
    const name = document.getElementById('modalUserName').value.trim();
    if (!name) { alert('Name is required.'); return; }
    const iconPath = document.getElementById('modalUserIconPath').value.trim() || null;
    if (!id) {
      api.users.create(name, iconPath).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalUser')).hide(); loadUsers(); }).catch(e => alert(e));
    } else {
      api.users.update(parseInt(id, 10), name, iconPath).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalUser')).hide(); loadUsers(); }).catch(e => alert(e));
    }
  });

  document.getElementById('modalTaskSave')?.addEventListener('click', saveTaskModal);

  document.getElementById('modalLabelSave')?.addEventListener('click', function () {
    const id = document.getElementById('modalLabelId').value;
    const name = document.getElementById('modalLabelName').value.trim();
    if (!name) { alert('Name is required.'); return; }
    const description = document.getElementById('modalLabelDescription').value.trim() || null;
    const color = document.getElementById('modalLabelColor').value || '#000000';
    if (!id) {
      api.labels.create(name, description, color).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalLabel')).hide(); loadLabels(); }).catch(e => alert(e));
    } else {
      api.labels.update(parseInt(id, 10), name, description, color).then(() => { bootstrap.Modal.getInstance(document.getElementById('modalLabel')).hide(); loadLabels(); }).catch(e => alert(e));
    }
  });

  document.getElementById('menuGroupCreate')?.addEventListener('click', function (e) { e.preventDefault(); document.getElementById('modalGroupTitle').textContent = 'Create Group'; document.getElementById('modalGroupId').value = ''; document.getElementById('modalGroupName').value = ''; document.getElementById('modalGroupParentWrap').style.display = 'block'; loadGroupOptions('modalGroupParent'); new bootstrap.Modal(document.getElementById('modalGroup')).show(); });
  document.getElementById('menuGroupRename')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.selectedGroupId) { alert('Select a group first.'); return; } document.getElementById('modalGroupTitle').textContent = 'Rename Group'; api.groups.get(state.selectedGroupId).then(g => { if (!g) return; document.getElementById('modalGroupId').value = g.groupId; document.getElementById('modalGroupName').value = g.name; document.getElementById('modalGroupParentWrap').style.display = 'none'; new bootstrap.Modal(document.getElementById('modalGroup')).show(); }); });
  document.getElementById('menuGroupDelete')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.selectedGroupId) { alert('Select a group first.'); return; } if (!confirm('Delete this group?')) return; api.groups.delete(state.selectedGroupId).then(() => { state.selectedGroupId = null; loadGroupsTree(); loadGroupOptions('filterGroup'); loadGroupOptions('modalTaskGroup'); loadTasks(); }).catch(e => alert(e)); });

  document.getElementById('menuTaskCreate')?.addEventListener('click', function (e) { e.preventDefault(); openTaskModal(null); });
  document.getElementById('menuTaskEdit')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.selectedTaskId) { alert('Select a task first (click on a card).'); return; } openTaskModal(state.selectedTaskId); });
  document.getElementById('menuTaskDelete')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.selectedTaskId) { alert('Select a task first.'); return; } if (!confirm('Delete this task?')) return; api.tasks.delete(state.selectedTaskId).then(() => { state.selectedTaskId = null; loadTasks(); }).catch(e => alert(e)); });

  document.getElementById('menuUserCreate')?.addEventListener('click', function (e) { e.preventDefault(); document.getElementById('modalUserId').value = ''; document.getElementById('modalUserName').value = ''; document.getElementById('modalUserIconPath').value = ''; document.getElementById('modalUserTitle').textContent = 'Create User'; new bootstrap.Modal(document.getElementById('modalUser')).show(); });
    document.getElementById('menuUserEdit')?.addEventListener('click', function (e) { e.preventDefault(); const id = state.currentUserId || (state.users[0] && state.users[0].userId); if (!id) { alert('Select or create a user first.'); return; } api.users.get(id).then(u => { if (!u) return; document.getElementById('modalUserId').value = u.userId; document.getElementById('modalUserName').value = u.name; document.getElementById('modalUserIconPath').value = u.iconPath || ''; document.getElementById('modalUserTitle').textContent = 'Edit User'; new bootstrap.Modal(document.getElementById('modalUser')).show(); }); });
    document.getElementById('menuUserList')?.addEventListener('click', function (e) { e.preventDefault(); api.users.list().then(users => { const el = document.getElementById('listUsersContent'); el.innerHTML = Array.from(users).map(u => `<div class="d-flex align-items-center gap-2 py-2 border-bottom"><span>${escapeHtml(u.name)}</span><button class="btn btn-sm btn-outline-primary ms-auto" data-user-id="${u.userId}">Use as comment author</button></div>`).join(''); el.querySelectorAll('[data-user-id]').forEach(btn => btn.addEventListener('click', function () { state.currentUserId = parseInt(this.dataset.userId, 10); localStorage.setItem('smallTask_currentUserId', state.currentUserId); bootstrap.Modal.getInstance(document.getElementById('modalListUsers')).hide(); })); new bootstrap.Modal(document.getElementById('modalListUsers')).show(); }); });

  document.getElementById('menuLabelCreate')?.addEventListener('click', function (e) { e.preventDefault(); document.getElementById('modalLabelId').value = ''; document.getElementById('modalLabelName').value = ''; document.getElementById('modalLabelDescription').value = ''; document.getElementById('modalLabelColor').value = '#000000'; document.getElementById('modalLabelTitle').textContent = 'Create Label'; new bootstrap.Modal(document.getElementById('modalLabel')).show(); });
  document.getElementById('menuLabelEdit')?.addEventListener('click', function (e) { e.preventDefault(); if (!state.labels.length) { alert('Create a label first.'); return; } const l = state.labels[0]; document.getElementById('modalLabelId').value = l.labelId; document.getElementById('modalLabelName').value = l.name; document.getElementById('modalLabelDescription').value = l.description || ''; document.getElementById('modalLabelColor').value = l.color || '#000000'; document.getElementById('modalLabelTitle').textContent = 'Edit Label'; new bootstrap.Modal(document.getElementById('modalLabel')).show(); });
  document.getElementById('menuLabelList')?.addEventListener('click', function (e) { e.preventDefault(); api.labels.list().then(labels => { const el = document.getElementById('listLabelsContent'); el.innerHTML = labels.map(l => `<div class="d-flex align-items-center gap-2 py-2 border-bottom"><span class="task-card-label" style="background:${l.color}20;color:${l.color}">${escapeHtml(l.name)}</span><span class="text-muted small">${escapeHtml(l.description || '')}</span></div>`).join(''); new bootstrap.Modal(document.getElementById('modalListLabels')).show(); }); });

  document.getElementById('btnApplyFilter')?.addEventListener('click', function () { setStoredFilter(getCurrentFilter()); loadTasks(); });
  document.querySelectorAll('#filterArea input, #filterArea select').forEach(el => el.addEventListener('change', function () { loadTasks(); }));
  document.querySelectorAll('#filterMenu [data-filter="default"]').forEach(el => el.addEventListener('click', function (e) { e.preventDefault(); setFilterToInputs({}); setStoredFilter({}); loadTasks(); }));
  document.querySelectorAll('#filterMenu [data-filter="save"]').forEach(el => el.addEventListener('click', function (e) { e.preventDefault(); setStoredFilter(getCurrentFilter()); alert('Filter saved.'); }));
  document.querySelectorAll('#filterMenu [data-filter="delete"]').forEach(el => el.addEventListener('click', function (e) { e.preventDefault(); setStoredFilter({}); setFilterToInputs({}); loadTasks(); }));

  document.querySelectorAll('#statusMenuContainer [data-status]').forEach(el => el.addEventListener('click', function (e) { e.preventDefault(); const status = this.dataset.status; const taskId = state.selectedTaskId; document.getElementById('statusMenuContainer').style.display = 'none'; if (!taskId) return; if (status === 'delete') { if (!confirm('Delete this task?')) return; api.tasks.delete(taskId).then(() => loadTasks()); return; } api.tasks.get(taskId).then(t => { if (!t) return; api.tasks.update(taskId, { title: t.title, description: t.description, status: parseInt(status, 10), priority: t.priority, assignedUserId: t.assignedUserId, groupId: t.groupId, labelIds: (t.taskLabels || []).map(tl => tl.labelId) }).then(() => loadTasks()); }); }));

  document.addEventListener('click', function () { document.getElementById('statusMenuContainer').style.display = 'none'; });

  setFilterToInputs(state.filter);
  loadGroupsTree().then(() => { loadGroupOptions('filterGroup'); loadGroupOptions('modalTaskGroup'); });
  loadUsers();
  loadLabels().then(() => loadTasks());
})();
