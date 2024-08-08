const skippedProperties = ['isUpToUp', 'dartType', 'menu', 'skills', 'damInfo'];

const escapeHtml = (unsafe) => {
    return unsafe.replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&#039;');
}

document.addEventListener('DOMContentLoaded', function () {
    const allSideMenu = document.querySelectorAll('#sidebar .side-menu.top li a');
    const tableContainer = document.querySelector('#table-container');
    const paginationContainer = document.querySelector('#pagination');
    let currentPage = 1;
    const rowsPerPage = 10;
    let currentData = null;

    allSideMenu.forEach(item => {
        const li = item.parentElement;
        item.addEventListener('click', function () {
            allSideMenu.forEach(i => {
                i.parentElement.classList.remove('active');
            });
            li.classList.add('active');
            currentPage = 1;
            loadPageContent(item.dataset.page);
        });
    });

    function loadPageContent(page) {
        let url;
        let title;
        let breadcrumb;
        let searchContainer = '';
        switch (page) {
            case 'itemTemplates':
                url = 'HSNR/Server1/ItemTemplates.json';
                title = 'Item Templates';
                breadcrumb = 'Item Templates';
                searchContainer = `
                <div class="search-container">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
            case 'npcTemplates':
                url = 'HSNR/Server1/NpcTemplates.json';
                title = 'NPC Templates';
                breadcrumb = 'NPC Templates';
                searchContainer = `
                <div class="search-container">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
            case 'nClasses':
                url = 'HSNR/Server1/NClasses.json';
                title = 'Class Skills';
                breadcrumb = 'Class Skills';
                searchContainer = `
                <div class="search-container">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
            case 'itemOptions':
                url = 'HSNR/Server1/ItemOptionTemplates.json';
                title = 'Item Options';
                breadcrumb = 'Item Options';
                searchContainer = `
                    <div class="search-container">
                    <div class="search-container">
                        <input type="text" id="search-id" placeholder="Tìm kiếm theo số thứ tự...">
                <div class="search-container">
                        <input type="text" id="search-id" placeholder="Tìm kiếm theo số thứ tự...">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
            case 'maps':
                url = 'HSNR/Server1/Maps.json';
                title = 'Maps';
                breadcrumb = 'Maps';
                searchContainer = `
                    <div class="search-container">
                    <div class="search-container">
                        <input type="text" id="search-id" placeholder="Tìm kiếm theo số thứ tự...">
                <div class="search-container">
                        <input type="text" id="search-id" placeholder="Tìm kiếm theo số thứ tự...">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
        }
        document.getElementById('page-title').textContent = title;
        document.getElementById('breadcrumb-active').textContent = breadcrumb;

        fetchData(url, page, searchContainer);
    }

    function searchData() {
        const searchInput = document.getElementById('search-name');
        const searchValue = searchInput.value.toLowerCase();

        var filteredData = currentData.filter(item => {
            return item.name?.toString().toLowerCase().includes(searchValue);
        });

        currentPage = 1;
        displayPageData(filteredData);
    }

    function fetchData(url, page, searchContainer) {
        fetch(url)
            .then(response => response.json())
            .then(data => {
                if (page === 'nClasses') {
                    let data2;
                    data.forEach(item => {
                        if (!data2)
                            data2 = item.skillTemplates;
                        else
                            data2 = data2.concat(item.skillTemplates);
                    });
                    data = data2;
                    data.sort((a, b) => a.id - b.id);
                }
                currentData = data;
                let tableData = `<table id="data-table"><thead><tr>`;
                Object.getOwnPropertyNames(data[0]).forEach(prop => {
                    if (skippedProperties.includes(prop))
                        return;
                    tableData += `<th>${prop.charAt(0).toUpperCase() + prop.slice(1)}</th>`;
                });
                tableData += `</tr></thead><tbody></tbody></table>`;
                tableContainer.innerHTML = searchContainer + tableData;
                if (searchContainer)
                    tableContainer.querySelector('.search-container button').addEventListener('click', searchData);
                displayPageData(data);
            })
            .catch(error => console.error('Error loading data:', error));
    }

    function displayPageData(data) {
        const tableBody = document.querySelector('#data-table tbody');
        tableBody.innerHTML = '';

        if (Array.isArray(data)) {
            const startIndex = (currentPage - 1) * rowsPerPage;
            const endIndex = startIndex + rowsPerPage;
            const pageData = data.slice(startIndex, endIndex);

            pageData.forEach(item => {
                const row = document.createElement('tr');
                Object.getOwnPropertyNames(item).forEach(prop => {
                    if (skippedProperties.includes(prop))
                        return;
                    const cell = document.createElement('td');
                    cell.textContent = escapeHtml(item[prop].toString());
                    row.appendChild(cell);
                });
                tableBody.appendChild(row);
            });

            setupPagination(data.length);
        }
    }

    function setupPagination(totalItems) {
        const totalPages = Math.ceil(totalItems / rowsPerPage);
        paginationContainer.innerHTML = '';

        const prevButton = document.createElement('button');
        prevButton.textContent = '<<';
        prevButton.disabled = currentPage === 1;
        prevButton.addEventListener('click', () => {
            currentPage = 1;
            displayPageData(currentData);
        });
        paginationContainer.appendChild(prevButton);

        if (totalPages < 5) {
            for (let i = 1; i <= totalPages; i++) {
                const button = document.createElement('button');
                button.textContent = i;
                button.classList.toggle('active', i === currentPage);
                button.addEventListener('click', () => {
                    currentPage = i;
                    displayPageData(currentData);
                });
                paginationContainer.appendChild(button);
            }
        }
        else {
            let start = currentPage - 1;
            if (currentPage <= 2)
                start = 1;
            if (currentPage >= totalPages - 2)
                start = 1;
            for (let i = start; i <= start + 2; i++) {
                const button = document.createElement('button');
                button.textContent = i;
                button.classList.toggle('active', i === currentPage);
                button.addEventListener('click', () => {
                    currentPage = i;
                    displayPageData(currentData);
                });
                paginationContainer.appendChild(button);
            }
            const button = document.createElement('button');
            button.textContent = "...";
            button.disabled = true;
            paginationContainer.appendChild(button);
            for (let i = totalPages - 2; i <= totalPages; i++) {
                const button = document.createElement('button');
                button.textContent = i;
                button.classList.toggle('active', i === currentPage);
                button.addEventListener('click', () => {
                    currentPage = i;
                    displayPageData(currentData);
                });
                paginationContainer.appendChild(button);
            }
        }

        const nextButton = document.createElement('button');
        nextButton.textContent = '>>';
        nextButton.disabled = currentPage === totalPages;
        nextButton.addEventListener('click', () => {
            currentPage = totalPages;
            displayPageData(currentData);
        });
        paginationContainer.appendChild(nextButton);

        // const pageInfo = document.createElement('span');
        // pageInfo.className = 'page-info';
        // pageInfo.textContent = `Page ${currentPage} of ${totalPages}`;
        // paginationContainer.appendChild(pageInfo);
    }

    loadPageContent('itemTemplates');
});