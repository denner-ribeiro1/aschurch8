﻿
----EXEC dbo.SalvarParametroSistema
----	@Nome = 'DesligarIdentityMembro'
----	, @Descricao = 'Desligar campo Identity do campo Id da tabela de Membros para migração de dados'
----	, @Valor = '1'

---- 1 - Cadastrar Sede
--INSERT INTO dbo.Congregacao(Nome, Sede, DataCriacao, DataAlteracao, CongregacaoResponsavelId)
--SELECT 'Congregação Sede', 1, GETDATE(), GETDATE(), 1

--DECLARE @CongregacaoId INT 
--SELECT @CongregacaoId = SCOPE_IDENTITY()


---- 2 - Cadastrar Perfil
--INSERT INTO dbo.Perfil(Nome, TipoPerfil, Status, DataCriacao, DataAlteracao)
--SELECT 'Administrador', 1, 1, GETDATE(), GETDATE()

--DECLARE @PerfilAdmId INT 
--SELECT @PerfilAdmId = SCOPE_IDENTITY()

---- 3 - Cadastrar Usuario Inicial
--IF(@CongregacaoId > 0)
--BEGIN

--	INSERT INTO dbo.Usuario(Nome, Username, Senha, Email, AlterarSenhaProxLogin, DataCriacao, CongregacaoId, PerfilId)
--	SELECT	'Sistema', 'sistema', 'Pc80pgI2M6DZJSHsnI1a5A==', '', 0, GETDATE(), @CongregacaoId, @PerfilAdmId

--	--sistema:@dmin123 (Pc80pgI2M6DZJSHsnI1a5A==)
--END

--UPDATE dbo.Rotina
--SET
--	MenuDescricao = 'Cargos'
--WHERE SubMenuDescricao = 'Cargo'

--UPDATE dbo.Rotina
--SET
--	MenuDescricao = 'Usuários'
--WHERE SubMenuDescricao = 'Usuário'


--DELETE FROM dbo.Rotina
--WHERE ID NOT IN (1, 5, 11, 13)


--Inclusao dos Pais na tabela.
--INSERT INTO Pais(Nome, Abrev) SELECT 'Brasil', 'BR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Afeganistão', 'AF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'África do Sul', 'ZA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Albânia', 'AL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Alemanha', 'DE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Andorra', 'AD'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Angola', 'AO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Anguilla', 'AI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Antártica', 'AQ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Antígua e Barbuda', 'AG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Arábia Saudita', 'SA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Argélia', 'DZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Argentina', 'AR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Armênia', 'AM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Aruba', 'AW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Austrália', 'AU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Áustria', 'AT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Azerbaijão', 'AZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Bahamas', 'BS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Bangladesh', 'BD'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Barbados', 'BB'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Barém', 'BH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Belarus', 'BY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Bélgica', 'BE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Belize', 'BZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Benin', 'BJ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Bermuda', 'BM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Birmânia', 'MM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Bolívia', 'BO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Bósnia e Herzegovina', 'BA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Botswana', 'BW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Brunei', 'BN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Bulgária', 'BG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Burkina Faso', 'BF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Burundi', 'BI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Butão', 'BT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Cabo Verde', 'CV'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Camarões', 'CM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Camboja', 'KH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Canadá', 'CA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Cazaquistão', 'KZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Chad', 'TD'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Chile', 'CL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'China', 'CN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Chipre', 'CY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Cingapura', 'SG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Cisjordânia', 'PS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Colômbia', 'CO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Cômoros', 'KM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Coreia do Norte', 'KP'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Coreia do Sul', 'KR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Costa do Marfim', 'CI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Costa Rica', 'CR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Croácia', 'HR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Cuba', 'CU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Curaçao', 'CW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Dinamarca', 'DK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Djibuti', 'DJ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Dominica', 'DM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Egito', 'EG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'El Salvador', 'SV'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Emirados Árabes Unidos', 'AE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Equador', 'EC'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Eritréia', 'ER'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Eslováquia', 'SK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Eslovênia', 'SI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Espanha', 'ES'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Estados Unidos', 'US'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Estônia', 'EE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Etiópia', 'ET'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Faixa de Gaza', 'FG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Fiji', 'FJ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Filipinas', 'PH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Finlândia', 'FI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'França Metropolitana', 'FX'
--INSERT INTO Pais(Nome, Abrev) SELECT 'França', 'FR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Gabão', 'GA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Gâmbia', 'GM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Gana', 'GH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Geórgia do Sul e Ilhas', 'GS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Geórgia', 'GE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Gibraltar', 'GI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Grécia', 'GR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Greenland', 'GL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Grenada', 'GD'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guadelupe', 'GP'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guam', 'GU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guatemala', 'GT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guernsey', 'GG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guiana Francesa', 'GF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guiana', 'GY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guiné Equatorial', 'GQ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guiné', 'GN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Guiné-Bissau', 'GW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Haiti', 'HT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Holanda', 'NL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Honduras', 'HN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Hong Kong', 'HK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Hungria', 'HU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Iêmen', 'YE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilha Bouvet', 'BV'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilha Christmas', 'CX'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilha de Man', 'IM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilha Norfolk', 'NF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Caiman', 'KY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Cocos (Keeling)', 'CC'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Cook', 'CK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Falkland (Malvinas)', 'FK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Feroe', 'FO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Heard and McDonald', 'HM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Marianas do Norte', 'MP'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Marshall', 'MH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Maurício', 'MU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Menores Distantes dos Estados Unidos', 'UM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Pitcairn', 'PN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Salomão', 'SB'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Turks e Caicos', 'TC'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Virgens Americanas', 'VI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Virgens Britânicas', 'VG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ilhas Wallis e Futuna', 'WF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Índia', 'IN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Indonésia', 'ID'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Irã', 'IR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Iraque', 'IQ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Irlanda', 'IE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Islândia', 'IS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Israel', 'IL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Itália', 'IT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Jamaica', 'JM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Japão', 'JP'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Jersey', 'JE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Jordânia', 'JO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Kiribati', 'KI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Kosovo', 'XK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Kuwait', 'KW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Laos', 'LA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Lesoto', 'LS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Letônia', 'LV'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Líbano', 'LB'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Libéria', 'LR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Líbia', 'LY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Liechtenstein', 'LI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Lituânia', 'LT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Luxemburgo', 'LU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Macao', 'MO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Macedônia', 'MK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Madagascar', 'MG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Maiote', 'YT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Malásia', 'MY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Malawi', 'MW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Maldivas', 'MV'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Mali', 'ML'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Malta', 'MT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Marrocos', 'MA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Martinica', 'MQ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Mauritânia', 'MR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'México', 'MX'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Micronésia, Estados Federados da', 'FM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Moçambique', 'MZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Moldova', 'MD'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Mônaco', 'MC'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Mongólia', 'MN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Montenegro', 'ME'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Montserrat', 'MS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Namíbia', 'NA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Nauru', 'NR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Nepal', 'NP'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Nicarágua', 'NI'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Níger', 'NE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Nigéria', 'NG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Niue', 'NU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Noruega', 'NO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Nova Caledônia', 'NC'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Nova Zelândia', 'NZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Omã', 'OM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Palau', 'PW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Panamá', 'PA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Papua Nova Guiné', 'PG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Paquistão', 'PK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Paraguai', 'PY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Peru', 'PE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Polinésia Francesa', 'PF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Polônia', 'PL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Porto Rico', 'PR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Portugal', 'PT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Qatar', 'QA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Quênia', 'KE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Quirguistão', 'KG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Reino Unido', 'GB'
--INSERT INTO Pais(Nome, Abrev) SELECT 'República Centro-Africana', 'CF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'República Democrática do Congo', 'CD'
--INSERT INTO Pais(Nome, Abrev) SELECT 'República do Congo', 'CG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'República Dominicana', 'DO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'República Tcheca', 'CZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Reunião', 'RE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Romênia', 'RO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ruanda', 'RW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Rússia', 'RU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Saara Ocidental', 'EH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Saint Martin', 'MF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Saint Pierre e Miquelon', 'PM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Samoa Americana', 'AS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Samoa', 'WS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'San Marino', 'SM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Santa Helena, Ascensão e Tristão da Cunha', 'SH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Santa Lúcia', 'LC'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Santa Sé (Cidade do Vaticano)', 'VA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'São Bartolomeu', 'BL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'São Cristóvão e Nevis', 'KN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'São Martinho', 'SX'
--INSERT INTO Pais(Nome, Abrev) SELECT 'São Tomé e Prín', 'ST'
--INSERT INTO Pais(Nome, Abrev) SELECT 'São Vicente e Granadinas', 'VC'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Seicheles', 'SC'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Senegal', 'SN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Serra Leoa', 'SL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Sérvia', 'RS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Síria', 'SY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Somália', 'SO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Sri Lanka', 'LK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Suazilândia', 'SZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Sudão do Sul', 'SS'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Sudão', 'SD'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Suécia', 'SE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Suíça', 'CH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Sul da França e Antártica', 'TF'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Suriname', 'SR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Svalbard', 'SJ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Tadjiquistão', 'TJ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Tailândia', 'TH'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Taiwan', 'TW'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Tanzânia', 'TZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Território Britânico do Oceano Índico', 'IO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Timor-Leste', 'TL'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Togo', 'TG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Tonga', 'TO'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Toquelau', 'TK'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Trinidad e Tobago', 'TT'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Tunísia', 'TN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Turcomenistão', 'TM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Turquia', 'TR'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Tuvalu', 'TV'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Ucrânia', 'UA'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Uganda', 'UG'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Uruguai', 'UY'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Uzbequistão', 'UZ'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Vanuatu', 'VU'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Venezuela', 'VE'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Vietnã', 'VN'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Zâmbia', 'ZM'
--INSERT INTO Pais(Nome, Abrev) SELECT 'Zimbábue', 'ZW'