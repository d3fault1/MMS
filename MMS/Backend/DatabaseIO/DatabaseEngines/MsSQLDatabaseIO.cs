using MMS.DataModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MMS.Backend.DatabaseIO
{
    class MsSQLDatabaseIO : IDatabaseIO
    {
        string connectionString = Helper.GetConnectionString(false);
        string masterConnectionString = Helper.GetConnectionString(true);
        public DatabaseCheckResult CheckDatabaseValidity()
        {
            Logging.Debug($"Attempting database validity check.");
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT TOP 1 * FROM \"museum_node\", \"museum_nodelog\", \"museum_exhibit\", \"museum_zone\", \"museum_floor\", \"museum_nodestatus\"";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Close();
                    }
                }
                Logging.Debug("Database validity check passed successfully");
                return DatabaseCheckResult.OK;
            }
            catch (Exception e)
            {
                if (e is SqlException se)
                {

                    if (se.Number == 4060)
                    {
                        Logging.Debug("Database not found");
                        return DatabaseCheckResult.NotFound;
                    }
                    Logging.Debug("Database found. Checking Validity");
                    if (se.Number == 208)
                    {
                        Logging.Debug("Database not configured properly");
                        return DatabaseCheckResult.TableCorrupt;
                    }
                    Logging.Debug($"Database check error. Exception No. {se.Number} Exception: {e.Message}");
                }

                Logging.Debug($"Database check error. Exception: {e.Message}");
                return DatabaseCheckResult.Exception;
            }
        }

        public bool CreateDatabase()
        {
            try
            {
                string dbname = Globals.Config.DBName;
                Logging.Debug($"Attempting SQL Create Database {dbname}");
                string sql = $"CREATE DATABASE \"{dbname}\"";
                using (var conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                Logging.Debug("SQL Create Database successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL Create Database failed. {e.Message}");
                return false;
            }
        }

        public bool CreateTables()
        {
            string table = "";
            string sql = "";
            try
            {
                Logging.Debug($"Attempting Database Tables Creation");
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        table = "museum_floor";
                        sql = $"CREATE TABLE \"{table}\" (" +
                                     "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY, " +
                                     "\"name\" VARCHAR(100) NOT NULL, " +
                                     "\"description\" VARCHAR(600) NOT NULL, " +
                                     "\"is_active\" BIT NOT NULL, " +
                                     "\"image\" VARCHAR(100), " +
                                     "\"created_at\" BIGINT NOT NULL, " +
                                     "\"updated_at\" BIGINT NOT NULL)";
                        Logging.Debug($"Attempting SQL Create Table {table}");
                        command.CommandText = sql;
                        command.ExecuteNonQuery();

                        table = "museum_zone";
                        sql = $"CREATE TABLE \"{table}\" (" +
                                     "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY, " +
                                     "\"floor_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_floor(id), " +
                                     "\"name\" VARCHAR(100) NOT NULL, " +
                                     "\"description\" VARCHAR(600) NOT NULL, " +
                                     "\"is_active\" BIT NOT NULL, " +
                                     "\"image\" VARCHAR(100), " +
                                     "\"created_at\" BIGINT NOT NULL, " +
                                     "\"updated_at\" BIGINT NOT NULL)";
                        Logging.Debug($"Attempting SQL Create Table {table}");
                        command.CommandText = sql;
                        command.ExecuteNonQuery();

                        table = "museum_exhibit";
                        sql = $"CREATE TABLE \"{table}\" (" +
                                     "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY, " +
                                     "\"zone_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_zone(id), " +
                                     "\"name\" VARCHAR(100) NOT NULL, " +
                                     "\"description\" VARCHAR(600) NOT NULL, " +
                                     "\"is_active\" BIT NOT NULL, " +
                                     "\"is_exhibit_show\" BIT NOT NULL, " +
                                     "\"image\" VARCHAR(100), " +
                                     "\"created_at\" BIGINT NOT NULL, " +
                                     "\"updated_at\" BIGINT NOT NULL)";
                        Logging.Debug($"Attempting SQL Create Table {table}");
                        command.CommandText = sql;
                        command.ExecuteNonQuery();

                        table = "museum_node";
                        sql = $"CREATE TABLE \"{table}\" (" +
                                     "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY, " +
                                     "\"name\" VARCHAR(100), " +
                                     "\"node_name\" VARCHAR(100), " +
                                     "\"description\" VARCHAR(300), " +
                                     "\"ip\" VARCHAR(100) NOT NULL, " +
                                     "\"is_active\" BIT NOT NULL, " +
                                     "\"is_config\" BIT NOT NULL, " +
                                     "\"os_type\" VARCHAR(150), " +
                                     "\"mac_addr\" VARCHAR(100) UNIQUE NOT NULL, " +
                                     "\"port\" INT NOT NULL, " +
                                     "\"unique_reg_code\" VARCHAR(300) UNIQUE NOT NULL, " +
                                     "\"os_name\" VARCHAR(255), " +
                                     "\"os_arch\" VARCHAR(255), " +
                                     "\"total_disk_space\" FLOAT, " +
                                     "\"total_cpu\" FLOAT, " +
                                     "\"total_ram\" FLOAT, " +
                                     "\"exhibit_id\" BIGINT FOREIGN KEY REFERENCES museum_exhibit(id), " +
                                     "\"zone_id\" BIGINT FOREIGN KEY REFERENCES museum_zone(id), " +
                                     "\"floor_id\" BIGINT FOREIGN KEY REFERENCES museum_floor(id), " +
                                     "\"content_metadata\" VARCHAR(MAX), " +
                                     "\"pem_file\" VARCHAR(MAX), " +
                                     "\"heartbeat_rate\" INT NOT NULL, " +
                                     "\"image\" VARCHAR(100), " +
                                     "\"is_online\" BIT NOT NULL, " +
                                     "\"sequence_id\" INT NOT NULL, " +
                                     "\"is_audio_guide\" BIT NOT NULL, " +
                                     "\"category\" VARCHAR(100), " +
                                     "\"is_control_panel\" BIT NOT NULL, " +
                                     "\"created_at\" BIGINT NOT NULL, " +
                                     "\"updated_at\" BIGINT NOT NULL)";
                        Logging.Debug($"Attempting SQL Create Table {table}");
                        command.CommandText = sql;
                        command.ExecuteNonQuery();

                        table = "museum_nodestatus";
                        sql = $"CREATE TABLE \"{table}\" (" +
                                     "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY, " +
                                     "\"node_id\" BIGINT UNIQUE NOT NULL FOREIGN KEY REFERENCES museum_node(id), " +
                                     "\"version\" VARCHAR(25), " +
                                     "\"temperature\" FLOAT, " +
                                     "\"cpu_usage\" INT, " +
                                     "\"disk_space_usage\" INT, " +
                                     "\"ram_usage\" INT, " +
                                     "\"current_timestamp\" FLOAT, " +
                                     "\"current_video_name\" VARCHAR(500), " +
                                     "\"current_video_number\" INT, " +
                                     "\"current_video_status\" VARCHAR(100), " +
                                     "\"current_volume\" INT, " +
                                     "\"video_duration\" FLOAT, " +
                                     "\"total_videos\" INT, " +
                                     "\"video_list\" VARCHAR(MAX), " +
                                     "\"uptime\" FLOAT, " +
                                     "\"created_at\" BIGINT NOT NULL, " +
                                     "\"updated_at\" BIGINT NOT NULL)";
                        Logging.Debug($"Attempting SQL Create Table {table}");
                        command.CommandText = sql;
                        command.ExecuteNonQuery();

                        table = "museum_nodelog";
                        sql = $"CREATE TABLE \"{table}\" (" +
                                     "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY, " +
                                     "\"node_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_node(id), " +
                                     "\"temperature\" FLOAT, " +
                                     "\"uptime\" FLOAT, " +
                                     "\"cpu_usage\" INT, " +
                                     "\"disk_space_usage\" INT, " +
                                     "\"ram_usage\" INT, " +
                                     "\"version\" VARCHAR(25), " +
                                     "\"created_at\" BIGINT NOT NULL, " +
                                     "\"updated_at\" BIGINT NOT NULL)";
                        Logging.Debug($"Attempting SQL Create Table {table}");
                        command.CommandText = sql;
                        command.ExecuteNonQuery();

                        table = "museum_command";
                        sql = $"CREATE TABLE \"{table}\" (" +
                                    "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY, " +
                                    "\"command\" VARCHAR(100) NOT NULL, " +
                                    "\"is_enabled\" BIT NOT NULL, " +
                                    "\"created_at\" BIGINT NOT NULL, " +
                                    "\"updated_at\" BIGINT NOT NULL)";
                        Logging.Debug($"Attempting SQL Create Table {table}");
                        command.CommandText = sql;
                        command.ExecuteNonQuery();

                        table = "museum_commandlog";
                        sql = $"CREATE TABLE \"{table}\" (" +
                                    "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY, " +
                                    "\"command_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_command(id), " +
                                    "\"node_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_node(id), " +
                                    "\"status\" VARCHAR(100), " +
                                    "\"message\" VARCHAR(MAX), " +
                                    "\"created_at\" BIGINT NOT NULL, " +
                                    "\"updated_at\" BIGINT NOT NULL)";
                        Logging.Debug($"Attempting SQL Create Table {table}");
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
                Logging.Debug("SQL Create Table successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL Create Table failed on table {table}. {e.Message}");
                return false;
            }
        }

        public List<ExhibitModel> ReadExhibitData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_exhibit");
                List<ExhibitModel> retval = new List<ExhibitModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_exhibit";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ord;
                                var row = new ExhibitModel();
                                ord = reader.GetOrdinal("id");
                                row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("zone_id");
                                row.ZoneID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("name");
                                row.Name = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("description");
                                row.Description = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("is_active");
                                row.IsActive = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("is_exhibit_show");
                                row.IsExhibitShow = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("image");
                                row.Image = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("created_at");
                                row.CreatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                ord = reader.GetOrdinal("updated_at");
                                row.UpdatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                retval.Add(row);
                            }
                        }
                        reader.Close();
                    }
                }
                Logging.Debug("SQL SELECT FROM museum_exhibit successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_exhibit failed. {e.Message}");
                return null;
            }
        }

        public List<ZoneModel> ReadZoneData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_zone");
                List<ZoneModel> retval = new List<ZoneModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_zone";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ord;
                                var row = new ZoneModel();
                                ord = reader.GetOrdinal("id");
                                row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("floor_id");
                                row.FloorID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("name");
                                row.Name = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("description");
                                row.Description = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("is_active");
                                row.IsActive = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("image");
                                row.Image = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("created_at");
                                row.CreatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                ord = reader.GetOrdinal("updated_at");
                                row.UpdatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                retval.Add(row);
                            }
                        }
                        reader.Close();
                    }
                }
                Logging.Debug("SQL SELECT FROM museum_zone successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_zone failed. {e.Message}");
                return null;
            }
        }

        public List<FloorModel> ReadFloorData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_floor");
                List<FloorModel> retval = new List<FloorModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_floor";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ord;
                                var row = new FloorModel();
                                ord = reader.GetOrdinal("id");
                                row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("name");
                                row.Name = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("description");
                                row.Description = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("is_active");
                                row.IsActive = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("image");
                                row.Image = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("created_at");
                                row.CreatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                ord = reader.GetOrdinal("updated_at");
                                row.UpdatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                retval.Add(row);
                            }
                        }
                        reader.Close();
                    }
                }
                Logging.Debug("SQL SELECT FROM museum_floor successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_floor failed. {e.Message}");
                return null;
            }
        }

        public List<NodeModel> ReadNodeData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_node");
                List<NodeModel> retval = new List<NodeModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_node";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ord;
                                var row = new NodeModel();
                                ord = reader.GetOrdinal("id");
                                row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("name");
                                row.Name = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("node_name");
                                row.NodeName = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("description");
                                row.Description = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("ip");
                                row.IP = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("is_active");
                                row.IsActive = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("is_config");
                                row.IsConfig = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("os_type");
                                row.OSType = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("mac_addr");
                                row.MacAddress = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("port");
                                row.Port = reader.IsDBNull(ord) ? -1 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("unique_reg_code");
                                row.RegKey = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("os_name");
                                row.OSName = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("os_arch");
                                row.OSArchitecture = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("total_disk_space");
                                row.TotalDiskSpace = reader.IsDBNull(ord) ? 0 : reader.GetDouble(ord);
                                ord = reader.GetOrdinal("total_cpu");
                                row.TotalCPU = reader.IsDBNull(ord) ? 0 : reader.GetDouble(ord);
                                ord = reader.GetOrdinal("total_ram");
                                row.TotalRam = reader.IsDBNull(ord) ? 0 : reader.GetDouble(ord);
                                ord = reader.GetOrdinal("exhibit_id");
                                row.ExhibitID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("zone_id");
                                row.ZoneID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("floor_id");
                                row.FloorID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("content_metadata");
                                row.ContentMetadata = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("pem_file");
                                row.PEMFile = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("image");
                                row.Image = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("heartbeat_rate");
                                row.HeartbeatRate = reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("is_online");
                                row.IsOnline = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("sequence_id");
                                row.SequenceID = reader.IsDBNull(ord) ? -1 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("is_audio_guide");
                                row.IsAudioGuide = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("is_control_panel");
                                row.IsControlPanel = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("category");
                                row.Category = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("created_at");
                                row.CreatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                ord = reader.GetOrdinal("updated_at");
                                row.UpdatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                retval.Add(row);
                            }
                        }
                        reader.Close();
                    }
                }
                Logging.Debug("SQL SELECT FROM museum_node successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_node failed. {e.Message}");
                return null;
            }
        }

        public List<NodeCurrentStatusModel> ReadNodeStatusData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_nodestatus");
                List<NodeCurrentStatusModel> retval = new List<NodeCurrentStatusModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_nodestatus";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ord;
                                var row = new NodeCurrentStatusModel();
                                ord = reader.GetOrdinal("node_id");
                                row.NodeID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("version");
                                row.Version = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("temperature");
                                row.Temperature = reader.IsDBNull(ord) ? 0 : reader.GetDouble(ord);
                                ord = reader.GetOrdinal("cpu_usage");
                                row.ProcessorUsage = reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("disk_space_usage");
                                row.DiskSpaceUsage = reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("ram_usage");
                                row.RamUsage = reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("current_timestamp");
                                row.TimeStamp = reader.IsDBNull(ord) ? TimeSpan.Zero : TimeSpan.FromSeconds(reader.GetDouble(ord));
                                ord = reader.GetOrdinal("current_video_name");
                                row.VideoName = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("current_video_number");
                                row.VideoNumber = reader.IsDBNull(ord) ? -1 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("current_video_status");
                                row.VideoStatus = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("current_volume");
                                row.Volume = reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("video_duration");
                                row.VideoDuration = reader.IsDBNull(ord) ? 0 : reader.GetDouble(ord);
                                ord = reader.GetOrdinal("total_videos");
                                row.TotalVideos = reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
                                ord = reader.GetOrdinal("video_list");
                                row.VideoList = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("uptime");
                                row.Uptime = reader.IsDBNull(ord) ? TimeSpan.Zero : TimeSpan.FromSeconds(reader.GetDouble(ord));
                                ord = reader.GetOrdinal("created_at");
                                row.CreatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                ord = reader.GetOrdinal("updated_at");
                                row.UpdatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                retval.Add(row);
                            }
                        }
                        reader.Close();
                    }
                }
                Logging.Debug("SQL SELECT FROM museum_nodestatus successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_nodestatus failed. {e.Message}");
                return null;
            }
        }

        public List<CommandModel> ReadCommandData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_command");
                List<CommandModel> retval = new List<CommandModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_command";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ord;
                                var row = new CommandModel();
                                ord = reader.GetOrdinal("id");
                                row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("command");
                                row.Command = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("is_enabled");
                                row.IsEnabled = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                                ord = reader.GetOrdinal("created_at");
                                row.CreatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                ord = reader.GetOrdinal("updated_at");
                                row.UpdatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                retval.Add(row);
                            }
                        }
                        reader.Close();
                    }
                }
                Logging.Debug("SQL SELECT FROM museum_command successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_command failed. {e.Message}");
                return null;
            }
        }

        public List<CommandLogModel> ReadCommandLogData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_commandlog");
                List<CommandLogModel> retval = new List<CommandLogModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_commandlog";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ord;
                                var row = new CommandLogModel();
                                ord = reader.GetOrdinal("id");
                                row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("command_id");
                                row.CommandID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("node_id");
                                row.NodeID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                                ord = reader.GetOrdinal("status");
                                row.Status = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("message");
                                row.Message = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                                ord = reader.GetOrdinal("created_at");
                                row.CreatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                ord = reader.GetOrdinal("updated_at");
                                row.UpdatedAt = reader.IsDBNull(ord) ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(ord)).DateTime;
                                retval.Add(row);
                            }
                        }
                        reader.Close();
                    }
                }
                Logging.Debug("SQL SELECT FROM museum_commandlog successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_commandlog failed. {e.Message}");
                return null;
            }
        }

        public bool WriteNodeLogData(ref List<NodeLogModel> data, bool updateExisting = false)
        {
            try
            {
                if (updateExisting) Logging.Debug($"SQL Attempting UPDATE museum_nodelog");
                else Logging.Debug($"SQL Attempting INSERT INTO museum_nodelog");
                string sql = "";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (updateExisting) sql = $"UPDATE \"museum_nodelog\" SET " +
                            $"\"temperature\" = @temperature, " +
                            $"\"uptime\" = @uptime, \"cpu_usage\" = @cpu_usage, " +
                            $"\"disk_space_usage\" = @disk_space_usage, " +
                            $"\"ram_usage\" = @ram_usage, \"version\" = @version, " +
                            $"\"updated_at\" = @updated_at WHERE \"node_id\" = @node_id";
                    else sql = $"INSERT INTO \"museum_nodelog\" (" +
                        $"\"node_id\", \"temperature\", \"uptime\", \"cpu_usage\", \"disk_space_usage\", " +
                        $"\"ram_usage\", \"version\", \"created_at\", \"updated_at\") VALUES(@node_id, @temperature, " +
                        $"@uptime, @cpu_usage, @disk_space_usage, @ram_usage, @version, @created_at, @updated_at)";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("@node_id", data[i].NodeID));
                            command.Parameters.Add(new SqlParameter("@temperature", data[i].Temperature));
                            command.Parameters.Add(new SqlParameter("@uptime", data[i].Uptime.TotalSeconds));
                            command.Parameters.Add(new SqlParameter("@cpu_usage", data[i].ProcessorUsage));
                            command.Parameters.Add(new SqlParameter("@disk_space_usage", data[i].DiskSpaceUsage));
                            command.Parameters.Add(new SqlParameter("@ram_usage", data[i].RamUsage));
                            command.Parameters.Add(new SqlParameter("@version", data[i].Version));
                            data[i].UpdatedAt = DateTime.Now;
                            command.Parameters.Add(new SqlParameter("@updated_at", new DateTimeOffset(data[i].UpdatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            if (!updateExisting)
                            {
                                data[i].CreatedAt = DateTime.Now;
                                command.Parameters.Add(new SqlParameter("@created_at", new DateTimeOffset(data[i].CreatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            }
                            command.ExecuteNonQuery();
                        }
                    }
                }
                if (updateExisting) Logging.Debug("SQL UPDATE museum_nodelog successful");
                else Logging.Debug("SQL INSERT INTO museum_nodelog successful");
                return true;
            }
            catch (Exception e)
            {
                if (updateExisting) Logging.Debug("SQL UPDATE museum_nodelog successful");
                else Logging.Debug($"SQL INSERT INTO museum_nodelog failed. {e.Message}");
                return false;
            }
        }

        public bool WriteNodeStatusData(ref List<NodeModel> data, bool updateExisting = false)
        {
            try
            {
                if (updateExisting) Logging.Debug($"SQL Attempting UPDATE museum_nodestatus");
                else Logging.Debug($"SQL Attempting INSERT INTO museum_nodestatus");
                string sql = "";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (updateExisting) sql = $"UPDATE \"museum_nodestatus\" SET " +
                            $"\"temperature\" = @temperature, \"uptime\" = @uptime, \"cpu_usage\" = @cpu_usage, " +
                            $"\"disk_space_usage\" = @disk_space_usage, \"ram_usage\" = @ram_usage, " +
                            $"\"version\" = @version, \"current_timestamp\" = @current_timestamp, " +
                            $"\"current_video_name\" = @current_video_name, \"current_video_number\" = @current_video_number, " +
                            $"\"current_video_status\" = @current_video_status, \"current_volume\" = @current_volume, " +
                            $"\"video_duration\" = @video_duration, \"total_videos\" = @total_videos, " +
                            $"\"video_list\" = @video_list, \"updated_at\" = @updated_at WHERE \"node_id\" = @node_id";
                    else sql = $"INSERT INTO \"museum_nodestatus\" (" +
                        $"\"node_id\", \"temperature\", \"uptime\", \"cpu_usage\", \"disk_space_usage\", " +
                        $"\"ram_usage\", \"version\", \"current_timestamp\", \"current_video_name\", " +
                        $"\"current_video_number\", \"current_video_status\", \"current_volume\", " +
                        $"\"video_duration\", \"total_videos\", \"video_list\", \"created_at\", \"updated_at\") " +
                        $"VALUES(@node_id, @temperature, @uptime, @cpu_usage, @disk_space_usage, " +
                        $"@ram_usage, @version, @current_timestamp, @current_video_name, " +
                        $"@current_video_number, @current_video_status, @current_volume, " +
                        $"@video_duration, @total_videos, @video_list, @created_at, @updated_at)";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("@node_id", data[i].CurrentStatus.NodeID));
                            command.Parameters.Add(new SqlParameter("@temperature", data[i].CurrentStatus.Temperature));
                            command.Parameters.Add(new SqlParameter("@uptime", data[i].CurrentStatus.Uptime.TotalSeconds));
                            command.Parameters.Add(new SqlParameter("@cpu_usage", data[i].CurrentStatus.ProcessorUsage));
                            command.Parameters.Add(new SqlParameter("@disk_space_usage", data[i].CurrentStatus.DiskSpaceUsage));
                            command.Parameters.Add(new SqlParameter("@ram_usage", data[i].CurrentStatus.RamUsage));
                            command.Parameters.Add(new SqlParameter("@version", data[i].CurrentStatus.Version));
                            command.Parameters.Add(new SqlParameter("@current_timestamp", data[i].CurrentStatus.TimeStamp.TotalSeconds));
                            command.Parameters.Add(new SqlParameter("@current_video_name", data[i].CurrentStatus.VideoName));
                            command.Parameters.Add(new SqlParameter("@current_video_number", data[i].CurrentStatus.VideoNumber));
                            command.Parameters.Add(new SqlParameter("@current_video_status", data[i].CurrentStatus.VideoStatus));
                            command.Parameters.Add(new SqlParameter("@current_volume", data[i].CurrentStatus.Volume));
                            command.Parameters.Add(new SqlParameter("@video_duration", data[i].CurrentStatus.VideoDuration));
                            command.Parameters.Add(new SqlParameter("@total_videos", data[i].CurrentStatus.TotalVideos));
                            command.Parameters.Add(new SqlParameter("@video_list", data[i].CurrentStatus.VideoList));
                            data[i].UpdatedAt = DateTime.Now;
                            command.Parameters.Add(new SqlParameter("@updated_at", new DateTimeOffset(data[i].CurrentStatus.UpdatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            if (!updateExisting)
                            {
                                data[i].CreatedAt = DateTime.Now;
                                command.Parameters.Add(new SqlParameter("@created_at", new DateTimeOffset(data[i].CurrentStatus.CreatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            }
                            command.ExecuteNonQuery();
                        }
                    }
                }
                if (updateExisting) Logging.Debug("SQL UPDATE museum_nodestatus successful");
                else Logging.Debug("SQL INSERT INTO museum_nodestatus successful");
                return true;
            }
            catch (Exception e)
            {
                if (updateExisting) Logging.Debug($"SQL UPDATE museum_nodestatus failed. {e.Message}");
                else Logging.Debug($"SQL INSERT INTO museum_nodestatus failed. {e.Message}");
                return false;
            }
        }

        public bool WriteNodeData(ref List<NodeModel> data, bool updateExisting = false)
        {
            try
            {
                if (updateExisting) Logging.Debug($"SQL Attempting UPDATE museum_node");
                else Logging.Debug($"SQL Attempting INSERT INTO museum_node");
                string sql = "";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (updateExisting) sql = $"UPDATE \"museum_node\" SET " +
                            $"\"name\" = @name, \"node_name\" = @node_name, \"description\" = @description, " +
                            $"\"ip\" = @ip, \"is_active\" = @is_active, \"is_config\" = @is_config, " +
                            $"\"os_type\" = @os_type, \"mac_addr\" = @mac_addr, \"port\" = @port, " +
                            $"\"unique_reg_code\" = @unique_reg_code, \"os_name\" = @os_name, " +
                            $"\"os_arch\" = @os_arch, \"total_disk_space\" = @total_disk_space, " +
                            $"\"total_cpu\" = @total_cpu, \"total_ram\" = @total_ram, \"exhibit_id\" = @exhibit_id, " +
                            $"\"zone_id\" = @zone_id, \"floor_id\" = @floor_id, \"content_metadata\" = @content_metadata, " +
                            $"\"pem_file\" = @pem_file, \"heartbeat_rate\" = @heartbeat_rate, \"image\" = @image, " +
                            $"\"is_online\" = @is_online, \"sequence_id\" = @sequence_id, \"is_audio_guide\" = @is_audio_guide, " +
                            $"\"category\" = @category, \"is_control_panel\" = @is_control_panel, \"updated_at\" = @updated_at WHERE \"id\" = @id";
                    else sql = $"INSERT INTO \"museum_node\" (" +
                        $"\"name\", \"node_name\", \"description\", \"ip\", \"is_active\", " +
                        $"\"is_config\", \"os_type\", \"mac_addr\", \"port\", \"unique_reg_code\", " +
                        $"\"os_name\", \"os_arch\", \"total_disk_space\", \"total_cpu\", \"total_ram\", " +
                        $"\"exhibit_id\", \"zone_id\", \"floor_id\", \"content_metadata\", \"pem_file\", " +
                        $"\"heartbeat_rate\", \"image\", \"is_online\", \"sequence_id\", \"is_audio_guide\", " +
                        $"\"category\", \"is_control_panel\", \"created_at\", \"updated_at\") " +
                        $"VALUES(@name, @node_name, @description, @ip, @is_active, @is_config, " +
                        $"@os_type, @mac_addr, @port, @unique_reg_code, @os_name, @os_arch, " +
                        $"@total_disk_space, @total_cpu, @total_ram, @exhibit_id, @zone_id, " +
                        $"@floor_id, @content_metadata, @pem_file, @heartbeat_rate, @image, " +
                        $"@is_online, @sequence_id, @is_audio_guide, @category, @is_control_panel, " +
                        $"@created_at, @updated_at); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("@name", data[i].Name));
                            command.Parameters.Add(new SqlParameter("@node_name", data[i].NodeName));
                            command.Parameters.Add(new SqlParameter("@description", data[i].Description));
                            command.Parameters.Add(new SqlParameter("@ip", data[i].IP));
                            command.Parameters.Add(new SqlParameter("@is_active", data[i].IsActive));
                            command.Parameters.Add(new SqlParameter("@is_config", data[i].IsConfig));
                            command.Parameters.Add(new SqlParameter("@os_type", data[i].OSType));
                            command.Parameters.Add(new SqlParameter("@mac_addr", data[i].MacAddress));
                            command.Parameters.Add(new SqlParameter("@port", data[i].Port));
                            command.Parameters.Add(new SqlParameter("@unique_reg_code", data[i].RegKey));
                            command.Parameters.Add(new SqlParameter("@os_name", data[i].OSName));
                            command.Parameters.Add(new SqlParameter("@os_arch", data[i].OSArchitecture));
                            command.Parameters.Add(new SqlParameter("@total_disk_space", data[i].TotalDiskSpace));
                            command.Parameters.Add(new SqlParameter("@total_cpu", data[i].TotalCPU));
                            command.Parameters.Add(new SqlParameter("@total_ram", data[i].TotalRam));
                            command.Parameters.Add(new SqlParameter("@exhibit_id", data[i].ExhibitID == -1 ? (object)DBNull.Value : data[i].ExhibitID));
                            command.Parameters.Add(new SqlParameter("@zone_id", data[i].ZoneID == -1 ? (object)DBNull.Value : data[i].ZoneID));
                            command.Parameters.Add(new SqlParameter("@floor_id", data[i].FloorID == -1 ? (object)DBNull.Value : data[i].FloorID));
                            command.Parameters.Add(new SqlParameter("@content_metadata", data[i].ContentMetadata));
                            command.Parameters.Add(new SqlParameter("@pem_file", data[i].PEMFile));
                            command.Parameters.Add(new SqlParameter("@heartbeat_rate", data[i].HeartbeatRate));
                            command.Parameters.Add(new SqlParameter("@image", data[i].Image));
                            command.Parameters.Add(new SqlParameter("@is_online", data[i].IsOnline));
                            command.Parameters.Add(new SqlParameter("@sequence_id", data[i].SequenceID));
                            command.Parameters.Add(new SqlParameter("@is_audio_guide", data[i].IsAudioGuide));
                            command.Parameters.Add(new SqlParameter("@category", data[i].Category));
                            command.Parameters.Add(new SqlParameter("@is_control_panel", data[i].IsControlPanel));
                            data[i].UpdatedAt = DateTime.Now;
                            command.Parameters.Add(new SqlParameter("@updated_at", new DateTimeOffset(data[i].UpdatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            if (updateExisting)
                            {
                                command.Parameters.Add(new SqlParameter("@id", data[i].ID));
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                data[i].CreatedAt = DateTime.Now;
                                command.Parameters.Add(new SqlParameter("@created_at", new DateTimeOffset(data[i].CreatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                                data[i].ID = (long)command.ExecuteScalar();
                            }
                        }
                    }
                }
                if (updateExisting) Logging.Debug("SQL UPDATE museum_node successful");
                else Logging.Debug("SQL INSERT INTO museum_node successful");
                return true;
            }
            catch (Exception e)
            {
                if (updateExisting) Logging.Debug($"SQL UPDATE museum_node failed. {e.Message}");
                else Logging.Debug($"SQL INSERT INTO museum_node failed. {e.Message}");
                return false;
            }
        }

        public bool WriteFloorData(ref List<FloorModel> data, bool updateExisting = false)
        {
            try
            {
                if (updateExisting) Logging.Debug($"SQL Attempting UPDATE museum_floor");
                else Logging.Debug($"SQL Attempting INSERT INTO museum_floor");
                string sql = "";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (updateExisting) sql = $"UPDATE \"museum_floor\" SET " +
                            $"\"name\" = @name, \"description\" = @description, " +
                            $"\"is_active\" = @is_active, \"image\" = @image, " +
                            $"\"updated_at\" = @updated_at WHERE \"id\" = @id";
                    else sql = $"INSERT INTO \"museum_floor\" (" +
                        $"\"name\", \"description\", \"is_active\", \"image\", \"created_at\", \"updated_at\") " +
                        $"VALUES(@name, @description, @is_active, @image, @created_at, " +
                        $"@updated_at); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("@name", data[i].Name));
                            command.Parameters.Add(new SqlParameter("@description", data[i].Description));
                            command.Parameters.Add(new SqlParameter("@is_active", data[i].IsActive));
                            command.Parameters.Add(new SqlParameter("@image", data[i].Image));
                            data[i].UpdatedAt = DateTime.Now;
                            command.Parameters.Add(new SqlParameter("@updated_at", new DateTimeOffset(data[i].UpdatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            if (updateExisting)
                            {
                                command.Parameters.Add(new SqlParameter("@id", data[i].ID));
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                data[i].CreatedAt = DateTime.Now;
                                command.Parameters.Add(new SqlParameter("@created_at", new DateTimeOffset(data[i].CreatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                                data[i].ID = (long)command.ExecuteScalar();
                            }
                        }
                    }
                }
                if (updateExisting) Logging.Debug("SQL UPDATE museum_floor successful");
                else Logging.Debug("SQL INSERT INTO museum_floor successful");
                return true;
            }
            catch (Exception e)
            {
                if (updateExisting) Logging.Debug($"SQL UPDATE museum_floor failed. {e.Message}");
                else Logging.Debug($"SQL INSERT INTO museum_floor failed. {e.Message}");
                return false;
            }
        }

        public bool WriteZoneData(ref List<ZoneModel> data, bool updateExisting = false)
        {
            try
            {
                if (updateExisting) Logging.Debug($"SQL Attempting UPDATE museum_zone");
                else Logging.Debug($"SQL Attempting INSERT INTO museum_zone");
                string sql = "";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (updateExisting) sql = $"UPDATE \"museum_zone\" SET " +
                            $"\"name\" = @name, \"description\" = @description, " +
                            $"\"is_active\" = @is_active, \"image\" = @image, " +
                            $"\"floor_id\" = @floor_id, \"updated_at\" = @updated_at WHERE \"id\" = @id";
                    else sql = $"INSERT INTO \"museum_zone\" (" +
                        $"\"name\", \"description\", \"is_active\", \"image\", \"floor_id\", \"created_at\", \"updated_at\") " +
                        $"VALUES(@name, @description, @is_active, @image, @floor_id, " +
                        $"@created_at, @updated_at); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("@name", data[i].Name));
                            command.Parameters.Add(new SqlParameter("@description", data[i].Description));
                            command.Parameters.Add(new SqlParameter("@is_active", data[i].IsActive));
                            command.Parameters.Add(new SqlParameter("@image", data[i].Image));
                            command.Parameters.Add(new SqlParameter("@floor_id", data[i].FloorID));
                            data[i].UpdatedAt = DateTime.Now;
                            command.Parameters.Add(new SqlParameter("@updated_at", new DateTimeOffset(data[i].UpdatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            if (updateExisting)
                            {
                                command.Parameters.Add(new SqlParameter("@id", data[i].ID));
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                data[i].CreatedAt = DateTime.Now;
                                command.Parameters.Add(new SqlParameter("@created_at", new DateTimeOffset(data[i].CreatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                                data[i].ID = (long)command.ExecuteScalar();
                            }
                        }
                    }
                }
                if (updateExisting) Logging.Debug("SQL UPDATE museum_zone successful");
                else Logging.Debug("SQL INSERT INTO museum_zone successful");
                return true;
            }
            catch (Exception e)
            {
                if (updateExisting) Logging.Debug($"SQL UPDATE museum_zone failed. {e.Message}");
                else Logging.Debug($"SQL INSERT INTO museum_zone failed. {e.Message}");
                return false;
            }
        }

        public bool WriteExhibitData(ref List<ExhibitModel> data, bool updateExisting = false)
        {
            try
            {
                if (updateExisting) Logging.Debug($"SQL Attempting UPDATE museum_exhibit");
                else Logging.Debug($"SQL Attempting INSERT INTO museum_exhibit");
                string sql = "";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (updateExisting) sql = $"UPDATE \"museum_exhibit\" SET " +
                            $"\"name\" = @name, \"description\" = @description, " +
                            $"\"is_active\" = @is_active, \"is_exhibit_show\" = @is_exhibit_show, " +
                            $"\"image\" = @image, \"zone_id\" = @zone_id, \"updated_at\" = @updated_at WHERE \"id\" = @id";
                    else sql = $"INSERT INTO \"museum_exhibit\" (" +
                        $"\"name\", \"description\", \"is_active\", \"is_exhibit_show\", " +
                        $"\"image\", \"zone_id\", \"created_at\", \"updated_at\") " +
                        $"VALUES(@name, @description, @is_active, @is_exhibit_show, @image, " +
                        $"@zone_id, @created_at, @updated_at); SELECT CONVERT(BIGINT, SCOPE_IDENTITY());";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("@name", data[i].Name));
                            command.Parameters.Add(new SqlParameter("@description", data[i].Description));
                            command.Parameters.Add(new SqlParameter("@is_active", data[i].IsActive));
                            command.Parameters.Add(new SqlParameter("@is_exhibit_show", data[i].IsExhibitShow));
                            command.Parameters.Add(new SqlParameter("@image", data[i].Image));
                            command.Parameters.Add(new SqlParameter("@zone_id", data[i].ZoneID));
                            data[i].UpdatedAt = DateTime.Now;
                            command.Parameters.Add(new SqlParameter("@updated_at", new DateTimeOffset(data[i].UpdatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            if (updateExisting)
                            {
                                command.Parameters.Add(new SqlParameter("@id", data[i].ID));
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                data[i].CreatedAt = DateTime.Now;
                                command.Parameters.Add(new SqlParameter("@created_at", new DateTimeOffset(data[i].CreatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                                data[i].ID = (long)command.ExecuteScalar();
                            }
                        }
                    }
                }
                if (updateExisting) Logging.Debug("SQL UPDATE museum_exhibit successful");
                else Logging.Debug("SQL INSERT INTO museum_exhibit successful");
                return true;
            }
            catch (Exception e)
            {
                if (updateExisting) Logging.Debug($"SQL UPDATE museum_exhibit failed. {e.Message}");
                else Logging.Debug($"SQL INSERT INTO museum_exhibit failed. {e.Message}");
                return false;
            }
        }

        public bool WriteCommandData(ref List<CommandModel> data, bool updateExisting = false)
        {
            try
            {
                if (updateExisting) Logging.Debug($"SQL Attempting UPDATE museum_command");
                else Logging.Debug($"SQL Attempting INSERT INTO museum_command");
                string sql = "";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (updateExisting) sql = $"UPDATE \"museum_command\" SET " +
                            $"\"command\" = @command, \"is_enabled\" = @is_enabled, " +
                            $"\"updated_at\" = @updated_at WHERE \"id\" = @id";
                    else sql = $"INSERT INTO \"museum_command\" (" +
                            $"\"command\", \"is_enabled\", \"created_at\", \"updated_at\") " +
                            $"VALUES(@command, @is_enabled, @created_at, @updated_at); " +
                            $"SELECT CONVERT(BIGINT, SCOPE_IDENTITY());";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("@command", data[i].Command));
                            command.Parameters.Add(new SqlParameter("@is_enabled", data[i].IsEnabled));
                            data[i].UpdatedAt = DateTime.Now;
                            command.Parameters.Add(new SqlParameter("@updated_at", new DateTimeOffset(data[i].UpdatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            if (updateExisting)
                            {
                                command.Parameters.Add(new SqlParameter("@id", data[i].ID));
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                data[i].CreatedAt = DateTime.Now;
                                command.Parameters.Add(new SqlParameter("@created_at", new DateTimeOffset(data[i].CreatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                                data[i].ID = (long)command.ExecuteScalar();
                            }
                        }
                    }
                }
                if (updateExisting) Logging.Debug("SQL UPDATE museum_command successful");
                else Logging.Debug("SQL INSERT INTO museum_command successful");
                return true;
            }
            catch (Exception e)
            {
                if (updateExisting) Logging.Debug($"SQL UPDATE museum_command failed. {e.Message}");
                else Logging.Debug($"SQL INSERT INTO museum_command failed. {e.Message}");
                return false;
            }
        }

        public bool WriteCommandLogData(ref List<CommandLogModel> data, bool updateExisting = false)
        {
            try
            {
                if (updateExisting) Logging.Debug($"SQL Attempting UPDATE museum_commandlog");
                else Logging.Debug($"SQL Attempting INSERT INTO museum_commandlog");
                string sql = "";
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (updateExisting) sql = $"UPDATE \"museum_commandlog\" SET " +
                            $"\"status\" = @status, \"message\" = @message, " +
                            $"\"updated_at\" = @updated_at WHERE \"id\" = @id";
                    else sql = $"INSERT INTO \"museum_commandlog\" (" +
                            $"\"command_id\", \"node_id\", \"status\", \"message\", \"created_at\", \"updated_at\") " +
                            $"VALUES(@command_id, @node_id, @status, @message, @created_at, @updated_at); " +
                            $"SELECT CONVERT(BIGINT, SCOPE_IDENTITY());";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add(new SqlParameter("@status", data[i].Status));
                            command.Parameters.Add(new SqlParameter("@message", data[i].Message));
                            data[i].UpdatedAt = DateTime.Now;
                            command.Parameters.Add(new SqlParameter("@updated_at", new DateTimeOffset(data[i].UpdatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                            if (updateExisting)
                            {
                                command.Parameters.Add(new SqlParameter("@id", data[i].ID));
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                command.Parameters.Add(new SqlParameter("@command_id", data[i].CommandID));
                                command.Parameters.Add(new SqlParameter("@node_id", data[i].NodeID));
                                data[i].CreatedAt = DateTime.Now;
                                command.Parameters.Add(new SqlParameter("@created_at", new DateTimeOffset(data[i].CreatedAt.ToUniversalTime()).ToUnixTimeSeconds()));
                                data[i].ID = (long)command.ExecuteScalar();
                            }
                        }
                    }
                }
                if (updateExisting) Logging.Debug("SQL UPDATE museum_commandlog successful");
                else Logging.Debug("SQL INSERT INTO museum_commandlog successful");
                return true;
            }
            catch (Exception e)
            {
                if (updateExisting) Logging.Debug($"SQL UPDATE museum_commandlog failed. {e.Message}");
                else Logging.Debug($"SQL INSERT INTO museum_commandlog failed. {e.Message}");
                return false;
            }
        }
    }
}
