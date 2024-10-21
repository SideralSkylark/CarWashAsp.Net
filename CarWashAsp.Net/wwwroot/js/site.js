const modal = document.getElementById("appointmentModal");
const newAppointmentBtn = document.getElementById("newAppointmentBtn");
const closeModalBtn = document.getElementById("closeModal");
const appointmentForm = document.getElementById("appointmentForm");
const appointmentTable = document.getElementById("appointmentTable").getElementsByTagName('tbody')[0];
let editingRow = null;

// Função para abrir o modal
newAppointmentBtn.addEventListener("click", function () {
    editingRow = null;  // Indica que é uma nova adição
    modal.style.display = "block";
    appointmentForm.reset();  // Limpa o formulário ao abrir o modal
});

// Função para fechar o modal
closeModalBtn.addEventListener("click", function () {
    modal.style.display = "none";
});

// Função para fechar o modal clicando fora dele
window.onclick = function (event) {
    if (event.target === modal) {
        modal.style.display = "none";
    }
};

// Função para adicionar ou editar agendamento
appointmentForm.addEventListener("submit", function (e) {
    e.preventDefault();

    const nomeCliente = document.getElementById("nomeCliente").value;
    const placaCarro = document.getElementById("placaCarro").value;
    const data = document.getElementById("data").value;
    const servico = document.getElementById("servico").value;
    const plano = document.getElementById("plano").value;
    const preco = document.getElementById("preco").value;

    const precoNumero = parseFloat(preco.replace('MZ', '').replace(',', '.'));

    const dataAtualizada = {
        nomeCliente: nomeCliente,
        placaCarro: placaCarro,
        data: formatDateToCustomFormat(data),
        plano: plano,
        servico: servico,
        preco: precoNumero
    };

    console.log("data: " + dataAtualizada.data);
    const url = editingRow ? "/Dashboard/EditarAgendamento" : "/Dashboard/AdicionarAgendamento";

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: new URLSearchParams(dataAtualizada)
    })
        .then(response => {
            if (response.ok) {
                if (editingRow) {
                    const updatedRow = editingRow;
                    updatedRow.cells[0].textContent = nomeCliente;
                    updatedRow.cells[1].textContent = plano;
                    updatedRow.cells[2].textContent = servico;
                    updatedRow.cells[3].textContent = placaCarro;
                    updatedRow.cells[4].textContent = data;
                    updatedRow.cells[5].textContent = `${precoNumero.toFixed(2)} MZ`;
                } else {
                    location.reload(); 
                }
            } else {
                alert('Erro ao salvar agendamento!');
            }
        })
        .catch(error => console.error('Erro:', error));

    modal.style.display = "none";
});

// Função para editar agendamentos
function editAppointment(button) {
    editingRow = button.parentElement.parentElement;
    console.log(editingRow);

    const id = editingRow.getAttribute('data-id');
    const nomeCliente = editingRow.cells[0].textContent;
    const plano = editingRow.cells[1].textContent;
    const servico = editingRow.cells[2].textContent;
    const placaCarro = editingRow.cells[3].textContent; 
    const data = editingRow.cells[4].textContent;
    const preco = editingRow.cells[5].textContent.replace('MZ', '').trim();


    document.getElementById("idAgendamento").value = id;
    document.getElementById("nomeCliente").value = nomeCliente;
    document.getElementById("placaCarro").value = placaCarro; 
    document.getElementById("data").value = data;
    document.getElementById("servico").value = servico;
    document.getElementById("plano").value = plano;
    document.getElementById("preco").value = preco;

    console.log("nome: " + nomeCliente + ", placa: " + placaCarro + ", data: " + data + ", servico: " + servico + ", plano: " + plano + ", preco: " + preco);

    modal.style.display = "block";
}

// Função para deletar agendamentos
function deleteAppointment(button) {
    if (confirm("Tem certeza que deseja cancelar o agendamento?")) {
        const row = button.parentElement.parentElement;
        const id = row.dataset.id;

        fetch('/Dashboard/DeletarAgendamento', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: new URLSearchParams({ id: id })
        })
            .then(response => response.ok ? row.remove() : alert('Erro ao deletar agendamento!'))
            .catch(error => console.error('Erro:', error));
    }
}

// Atualizar preço ao mudar serviço ou plano
function updatePreco() {
    const tipoServico = document.getElementById("servico").value;
    const plano = document.getElementById("plano").value;

    if (tipoServico && plano) {
        fetch(`/Dashboard/ObterPreco?tipoServico=${tipoServico}&plano=${plano}`)
            .then(response => response.json())
            .then(data => {
                if (data.preco) {
                    const preco = parseFloat(data.preco);
                    if (!isNaN(preco)) {
                        document.getElementById("preco").value = `${preco.toFixed(2)} MZ`;
                    } else {
                        console.error('Preço inválido recebido:', data.preco);
                        document.getElementById("preco").value = "Preço inválido";
                    }
                } else {
                    console.error('Preço não disponível.');
                    document.getElementById("preco").value = "Preço não disponível";
                }
            })
            .catch(error => console.error('Erro ao buscar o preço:', error));
    }
}

document.getElementById("servico").addEventListener("change", updatePreco);
document.getElementById("plano").addEventListener("change", updatePreco);

function formatDateToCustomFormat(dateString) {
    const date = new Date(dateString);

    const year = date.getFullYear(); // Ano completo
    const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Mês com dois dígitos
    const day = date.getDate().toString().padStart(2, '0'); // Dia com dois dígitos

    // Retorna a data no formato "yyyy/MM/dd"
    return `${year}/${month}/${day}`;
}


 

 // Função para abrir o modal
 function openProductModal() {
    document.getElementById('productModal').style.display = 'block';
}

// Função para fechar o modal
document.getElementById('closeModal').onclick = function () {
    document.getElementById('productModal').style.display = 'none';
}

// Função para abrir o modal de novo produto
document.getElementById('newProductBtn').onclick = function () {
    document.getElementById('productForm').reset();
    document.getElementById('modalTitle').textContent = 'Adicionar Produto';
    document.getElementById('idProduto').value = ''; // Limpa o campo oculto de ID
    openProductModal();
};

// Função para abrir o modal de edição de produto e preencher os dados
function editProduct(button) {
    var row = button.closest('tr'); // Obtém a linha do produto a partir do botão
    var id = row.getAttribute('data-id'); // Obtém o ID do produto da linha
    var tipoProduto = row.cells[0].textContent; // Obtém o tipo de produto
    var quantidade = row.cells[1].textContent; // Obtém a quantidade

    // Preenche os campos do formulário com os dados do produto
    document.getElementById('idProduto').value = id;
    document.getElementById('tipoProduto').value = tipoProduto;
    document.getElementById('quantidade').value = quantidade;

    // Atualiza o título do modal para indicar que é uma edição
    document.getElementById('modalTitle').textContent = 'Editar Produto';

    // Abre o modal
    openProductModal();
}

// Função para salvar o produto (adição ou edição)
document.getElementById('productForm').onsubmit = function (e) {
    e.preventDefault(); // Impede o envio tradicional do formulário

    var id = document.getElementById('idProduto').value;
    var tipoProduto = document.getElementById('tipoProduto').value;
    var quantidade = document.getElementById('quantidade').value;

    // Prepara os dados para enviar ao backend
    var produtoData = {
        id: id,
        tipoProduto: tipoProduto,
        quantidade: quantidade
    };

    // Determina a URL dependendo se é adição ou edição
    var url = id ? '/Dashboard/EditarProduto' : '/Dashboard/AdicionarProduto';

    // Faz a requisição ao backend para salvar o produto
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: new URLSearchParams(produtoData)
    })
    .then(response => response.json())
    .then(data => {
        if (id) {
            // Atualiza a linha do produto na tabela
            var row = document.querySelector('tr[data-id="' + id + '"]');
            row.cells[0].textContent = tipoProduto;
            row.cells[1].textContent = quantidade;
        } else {
            // Adiciona nova linha de produto
            var tbody = document.getElementById('productTable').getElementsByTagName('tbody')[0];
            var newRow = tbody.insertRow();
            newRow.setAttribute('data-id', data.id);
            newRow.insertCell(0).textContent = data.tipoProduto;
            newRow.insertCell(1).textContent = data.quantidade;
            var actionsCell = newRow.insertCell(2);
            var editButton = document.createElement('button');
            editButton.className = 'edit-btn';
            editButton.textContent = 'Editar';
            editButton.onclick = function () {
                editProduct(editButton);
            };
            actionsCell.appendChild(editButton);
        }
    })
    .catch(error => {
        console.error('Erro ao salvar o produto:', error);
    });

    // Fecha o modal após salvar
    document.getElementById('productModal').style.display = 'none';
};

// Função para remover produto
function removeProduct(id) {
    fetch('/Dashboard/RemoverProduto', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: new URLSearchParams({ id: id })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Remove a linha da tabela
            var row = document.querySelector('tr[data-id="' + id + '"]');
            row.remove();
        } else {
            alert('Erro ao remover o produto.');
        }
    })
    .catch(error => {
        console.error('Erro ao remover o produto:', error);
    });
}

// Função para carregar produtos do backend e renderizar na tabela
function loadProducts() {
    fetch('/Dashboard/ListarProdutos')
        .then(response => response.json())
        .then(data => {
            var tbody = document.getElementById('productTable').getElementsByTagName('tbody')[0];
            data.forEach(produto => {
                var row = tbody.insertRow();
                row.setAttribute('data-id', produto.id);
                row.insertCell(0).textContent = produto.tipoProduto;
                row.insertCell(1).textContent = produto.quantidade;
                var actionsCell = row.insertCell(2);
                var editButton = document.createElement('button');
                editButton.className = 'edit-btn';
                editButton.textContent = 'Editar';
                editButton.onclick = function () {
                    editProduct(editButton);
                };
                actionsCell.appendChild(editButton);
            });
        })
        .catch(error => console.error('Erro ao carregar produtos:', error));
}

// Carrega os produtos quando a página for aberta
document.addEventListener('DOMContentLoaded', loadProducts);