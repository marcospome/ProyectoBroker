using Microsoft.AspNetCore.Mvc;
using ProyectoProgramacion.Models;
using System.Data.SqlClient;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProyectoProgramacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrokerController : ControllerBase
    {

        public readonly string con;

        public BrokerController(IConfiguration configuration)
        {
            con = configuration.GetConnectionString("connection");
        }

        // Obtiene todas las ordenes cargadas en la base.
        [HttpGet("getAllOrders")]
        public IEnumerable<Actions> GetAllActions()
        {
            List<Actions> actions = new();
            using (SqlConnection connection = new(con)) // Utilizo la conexión declarada en el appsettings.json
            {
                connection.Open(); // Inicio la conexión
                using (SqlCommand cmd = new("GetOrder", connection)) // GetOrder, el PRC que hace un select * from a la tabla ORDERS_HISTORY por base.
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) // While para asegurarnos que se ejecute unicamente si levanta datos.
                        {
                            Actions A = new Actions // Creo el JSON con las variables que tiene que tomar. (DEBE TENER EL MISMO NOMBRE LA VARIABLE QUE POR BASE, RESPETANDO MAYUSCULAS, ETC.)
                            {
                                TxNumber = Convert.ToInt32(reader["TX_NUMBER"]),
                                OrderDate = Convert.ToDateTime(reader["ORDER_DATE"]),
                                Action = reader["ACTION"].ToString(),
                                Status = reader["STATUS"].ToString(),
                                Symbol = reader["SYMBOL"].ToString(),
                                Quantity = Convert.ToInt32(reader["QUANTITY"]),
                                Price = Convert.ToDecimal(reader["PRICE"])
                            };
                            actions.Add(A);
                        }
                    }
                }
                connection.Close(); // Finalizo la conexión
            }
            return actions;
        }

        /* Recuperar todos los datos de las órdenes “completadas”, incluido el
        monto neto de cada transacción (dada la cantidad de acciones y el
        precio) */

        [HttpGet("operationsStatusExecuted")]
        public List<Actions> GetOperationsStatusExecuted()
        {
            List<Actions> allActions = GetAllActions().ToList(); // Hago una lista con el metodo anterior que me trae todos los datos.

            List<Actions> filteredActions = allActions
            .Where(a => a.Status == "EXECUTED") // LINQ
            .ToList();

            return filteredActions;
        }

        // Recuperar todos los datos de las órdenes para un año específico.

        [HttpGet("operationsByYear")]
        public List<Actions> GetOperationsByYear(int year)
        {
            List<Actions> allActions = GetAllActions().ToList(); // Hago una lista con el metodo anterior que me trae todos los datos.

            List<Actions> filteredActions = allActions
            .Where(a => a.OrderDate.Year == year) // LINQ
            .ToList();


            return filteredActions;
        }


        [HttpPost("orderCreate")]
        public void CreateOrder([FromBody] Actions a)  // si pongo List<Action> puedo enviar varios elementos en un solo json.
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("CreateOrder", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ORDER_DATE", a.OrderDate);
                    cmd.Parameters.AddWithValue("@ACTION", a.Action);
                    cmd.Parameters.AddWithValue("@STATUS", a.Status);
                    cmd.Parameters.AddWithValue("@SYMBOL", a.Symbol);
                    cmd.Parameters.AddWithValue("@QUANTITY", a.Quantity);
                    cmd.Parameters.AddWithValue("@PRICE", a.Price);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /* Insertar una nueva orden de “COMPRA” o “VENTA” en la tabla, con el
        estado “PENDING”. */

        [HttpPost("orderCreatePending")]
        public void CreateOrderPending([FromBody] Actions a)  // si pongo List<Action> puedo enviar varios elementos en un solo json.
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("CreateOrder", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ORDER_DATE", a.OrderDate);
                    cmd.Parameters.AddWithValue("@ACTION", a.Action);
                    cmd.Parameters.AddWithValue("@STATUS", "PENDING");
                    cmd.Parameters.AddWithValue("@SYMBOL", a.Symbol);
                    cmd.Parameters.AddWithValue("@QUANTITY", a.Quantity);
                    cmd.Parameters.AddWithValue("@PRICE", a.Price);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }


        [HttpPut("orderEdit/{id}")]
        public void EditOrder([FromBody] Actions a, int id)  // si pongo List<Action> puedo enviar varios elementos en un solo json.
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("UpdateOrder", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@ORDER_DATE", a.OrderDate);
                    cmd.Parameters.AddWithValue("@ACTION", a.Action);
                    cmd.Parameters.AddWithValue("@STATUS", a.Status);
                    cmd.Parameters.AddWithValue("@SYMBOL", a.Symbol);
                    cmd.Parameters.AddWithValue("@QUANTITY", a.Quantity);
                    cmd.Parameters.AddWithValue("@PRICE", a.Price);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }



        [HttpDelete("orderDelete/{id}")]
        public void DeleteOrder(int id)  // si pongo List<Action> puedo enviar varios elementos en un solo json.
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                using (SqlCommand cmd = new("DeleteOrder", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}