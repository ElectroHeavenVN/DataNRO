const skippedProperties = ['isUpToUp', 'dartType', 'menu', 'skills', 'damInfo'];

const gamePublisher = 'TeaMobi';
const server = gamePublisher + '/Server8910';

document.addEventListener('DOMContentLoaded', function () {
    const allSideMenu = document.querySelectorAll('#sidebar .side-menu.top li a');
    const tableContainer = document.querySelector('#table-container');
    const paginationContainer = document.querySelector('#pagination');
    let currentPage = 1;
    const rowsPerPage = 10;
    let currentData = null;
    var filteredData = null;
    var parts = null;
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
                url = server + '/ItemTemplates.json';
                title = 'Items';
                breadcrumb = 'Items';
                searchContainer = `
                <div class="search-container">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
            case 'npcTemplates':
                url = server + '/NpcTemplates.json';
                title = 'NPCs';
                breadcrumb = 'NPCs';
                searchContainer = `
                <div class="search-container">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
            case 'nClasses':
                url = server + '/NClasses.json';
                title = 'Skills';
                breadcrumb = 'Skills';
                searchContainer = `
                <div class="search-container">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
            case 'itemOptions':
                url = server + '/ItemOptionTemplates.json';
                title = 'ItemOptions';
                breadcrumb = 'ItemOptions';
                searchContainer = `
                <div class="search-container">
                    <input type="text" id="search-name" placeholder="Tìm kiếm theo tên...">
                    <button>Tìm kiếm</button>
                </div>`;
                break;
            case 'maps':
                url = server + '/Maps.json';
                title = 'Maps';
                breadcrumb = 'Maps';
                searchContainer = `
                <div class="search-container">
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
        if (searchValue) {
            filteredData = currentData.filter(item => {
                return item.name?.toString().toLowerCase().includes(searchValue);
            });
            currentPage = 1;
            displayPageData(filteredData);
        } else {
            filteredData = null;
            currentPage = 1;
            displayPageData(currentData);
        }
    }
    document.querySelector('footer').innerHTML += decodeURIComponent(atob('ICAgICAgICA8cCBjbGFzcz0iZ2F5Ij4mY29weTsgMjAyNCBUcsaw4budbmcgR2lhbmcgKFZOR0FZKS4gQWxsIHJpZ2h0cyByZXNlcnZlZC48L3A+').split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));

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
                if (page === 'npcTemplates')
                    tableData += `<th>Image</th>`;
                tableData += `</tr></thead><tbody></tbody></table>`;
                tableContainer.innerHTML = searchContainer + tableData;
                if (searchContainer) {
                    var searchInput = tableContainer.querySelector('.search-container input');
                    searchInput.addEventListener('input', () => {
                        if (!searchInput.value.toLowerCase())
                            searchData();
                    });
                    tableContainer.querySelector('.search-container button').addEventListener('click', searchData);
                }
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

            pageData.forEach(npc => {
                const row = document.createElement('tr');
                Object.getOwnPropertyNames(npc).forEach(prop => {
                    if (skippedProperties.includes(prop))
                        return;
                    var cell = document.createElement('td');
                    if (prop.includes('icon')) {
                        var container = document.createElement('div');
                        container.classList.add('icon-container');
                        var img = document.createElement('img');
                        img.src = gamePublisher + '/Icons/' + npc[prop].toString() + '.png';
                        img.onerror = () => {
                            container.removeChild(img);
                            var text = document.createElement('span');
                            text.textContent = npc[prop];
                            container.appendChild(text);
                        }
                        container.appendChild(img);
                        cell.appendChild(container);
                    }
                    else
                        cell.textContent = npc[prop];
                    row.appendChild(cell);
                });
                if (Object.getOwnPropertyNames(npc).find(prop => prop.includes('npcTemplateId'))) {
                    if (npc.npcTemplateId === 3)
                    {
                        var cell = document.createElement('td');
                        var container = document.createElement('div');
                        container.classList.add('icon-container');
                        var img = document.createElement('img');
                        img.src = gamePublisher + '/Icons/265.png';
                        img.onerror = () => {
                            container.removeChild(img);
                            var text = document.createElement('span');
                            text.textContent = "265";
                            container.appendChild(text);
                        }
                        container.appendChild(img);
                        cell.appendChild(container);
                        row.appendChild(cell);
                    }
                    else if (npc.npcTemplateId === 4)
                        row.appendChild(document.createElement('td'));
                    else if (npc.npcTemplateId === 6)
                        {
                            var cell = document.createElement('td');
                            var container = document.createElement('div');
                            container.classList.add('icon-container');
                            var img = document.createElement('img');
                            img.src = gamePublisher + '/Icons/545.png';
                            img.onerror = () => {
                                container.removeChild(img);
                                var text = document.createElement('span');
                                text.textContent = "545";
                                container.appendChild(text);
                            }
                            container.appendChild(img);
                            cell.appendChild(container);
                            row.appendChild(cell);
                        }
                    else
                        LoadParts(npc, row);
                }
                tableBody.appendChild(row);
            });

            setupPagination(data);
        }
    }

    function LoadParts(npc, row) {
        var cell = document.createElement('td');
        var container = document.createElement('div');
        var childContainer = document.createElement('div');
        container.classList.add('npc-image-container');
        var partHeadId = npc.headId == -1 ? -1 : parts[npc.headId].pi[0].id;
        var partBodyId = npc.bodyId == -1 ? -1 : parts[npc.bodyId].pi[1].id;
        var partLegId = npc.legId == -1 ? -1 : parts[npc.legId].pi[1].id;
        var imgHead, imgBody, imgLeg;

        if (partLegId !== -1) {
            imgLeg = document.createElement('img');
            imgLeg.classList.add('npc-leg');
            imgLeg.src = gamePublisher + '/Icons/' + partLegId + '.png';
        }
        if (partBodyId !== -1) {
            imgBody = document.createElement('img');
            imgBody.classList.add('npc-body');
            imgBody.src = gamePublisher + '/Icons/' + partBodyId + '.png';
        }
        if (partHeadId !== -1) {
            imgHead = document.createElement('img');
            imgHead.classList.add('npc-head');
            imgHead.src = gamePublisher + '/Icons/' + partHeadId + '.png';
        }
        var baseTranslateX = 0, baseTranslateY = 0;
        if (imgHead) {
            imgHead.style.transform = "translateX(50px)";
            baseTranslateX = 50 + (-12 + parts[npc.headId].pi[0].dx) * 2;
            baseTranslateY = (-8 + parts[npc.headId].pi[0].dy) * 2;
        }
        if (imgBody) {
            imgBody.style.transform = `translate(${-baseTranslateX + 100 + (-8 + parts[npc.bodyId].pi[1].dx) * 2}px, ${-baseTranslateY + (10 + parts[npc.bodyId].pi[1].dy) * 2}px)`;
        }
        if (imgLeg) {
            imgLeg.style.transform = `translate(${-baseTranslateX + 100 + (-7 + parts[npc.legId].pi[1].dx) * 2}px, ${-baseTranslateY + (16 + parts[npc.legId].pi[1].dy) * 2}px)`;
        }

        if (imgLeg)
            childContainer.appendChild(imgLeg);
        if (imgHead)
            childContainer.appendChild(imgHead);
        if (imgBody)
            childContainer.appendChild(imgBody);

        CalcRowHeight(row, imgHead, imgBody, imgLeg).then(() => {
            container.appendChild(childContainer);
            cell.appendChild(container);
            row.appendChild(cell);
        });
    }

    async function CalcRowHeight(row, imgHead, imgBody, imgLeg) {
        var rowHeight = 0;
        if (imgBody) {
            await imgBody.decode();
            rowHeight += imgBody.height;
        }
        if (imgLeg) {
            await imgLeg.decode();
            rowHeight += imgLeg.height;
        }
        if (imgHead) {
            await imgHead.decode();
            rowHeight += imgHead.height;
        }
        if (rowHeight > 0)
            row.style.height = (rowHeight * 1.3) + 'px';
    }

    function setupPagination(data) {
        const totalPages = Math.ceil(data.length / rowsPerPage);
        paginationContainer.innerHTML = '';

        const firstButton = document.createElement('button');
        firstButton.textContent = '<<';
        firstButton.disabled = currentPage === 1;
        firstButton.addEventListener('click', () => {
            currentPage = 1;
            displayPageData(data);
        });
        paginationContainer.appendChild(firstButton);

        const prevButton = document.createElement('button');
        prevButton.textContent = '<';
        prevButton.disabled = currentPage === 1;
        prevButton.addEventListener('click', () => {
            if (currentPage > 1)
                currentPage--;
            displayPageData(data);
        });
        paginationContainer.appendChild(prevButton);

        if (totalPages < 5) {
            for (let i = 1; i <= totalPages; i++) {
                const button = document.createElement('button');
                button.textContent = i;
                button.classList.toggle('active', i === currentPage);
                button.addEventListener('click', () => {
                    currentPage = i;
                    displayPageData(data);
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
            if (start + 2 === totalPages - 2)
                start--;
            for (let i = start; i <= start + 2; i++) {
                const button = document.createElement('button');
                button.textContent = i;
                button.classList.toggle('active', i === currentPage);
                button.addEventListener('click', () => {
                    currentPage = i;
                    displayPageData(data);
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
                    displayPageData(data);
                });
                paginationContainer.appendChild(button);
            }
        }

        const nextButton = document.createElement('button');
        nextButton.textContent = '>';
        nextButton.disabled = currentPage === totalPages;
        nextButton.addEventListener('click', () => {
            if (currentPage < totalPages)
                currentPage++;
            displayPageData(currentData);
        });
        paginationContainer.appendChild(nextButton);

        const lastButton = document.createElement('button');
        lastButton.textContent = '>>';
        lastButton.disabled = currentPage === totalPages;
        lastButton.addEventListener('click', () => {
            currentPage = totalPages;
            displayPageData(currentData);
        });
        paginationContainer.appendChild(lastButton);

        // const pageInfo = document.createElement('span');
        // pageInfo.className = 'page-info';
        // pageInfo.textContent = `Page ${currentPage} of ${totalPages}`;
        // paginationContainer.appendChild(pageInfo);
    }

    fetch(server + "/Parts.json").then(response => response.json()).then(data => parts = data).catch(error => console.error('Error loading data:', error));
    loadPageContent('itemTemplates');
});