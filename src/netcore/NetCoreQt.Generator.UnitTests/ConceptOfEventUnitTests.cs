namespace NetCoreQt.Generator.UnitTests {

    public class ConceptOfEventUnitTests {

        [Fact]
        public void Create_Completed_SingleEvent () {
            //arrange

            //act
            var externalId = ConveptOfEventExternal.Create ( new ConceptOfEvent { IntValue = 5 } );

            //assert
            Assert.Equal ( 5, ConveptOfEventExternal.m_events[externalId].IntValue );
        }

        [Fact]
        public void Create_Failed_TryToCreateWithExistsIdentifier () {
            //arrange
            var counterId = ConveptOfEventExternal.m_counter;
            ConveptOfEventExternal.Create ( new ConceptOfEvent { IntValue = 9 } );
            ConveptOfEventExternal.m_counter -= 1;

            //assert
            Assert.Throws<Exception> (
                () => {
                    //act
                    ConveptOfEventExternal.Create ( new ConceptOfEvent { IntValue = 10 } );
                }
            );
        }

        [Fact]
        public void Create_Completed_GetIntValue () {
            //arrange
            var externalId = ConveptOfEventExternal.Create ( new ConceptOfEvent { IntValue = 20 } );

            //act
            var value = ConveptOfEventExternal.GetIntValue ( externalId );

            //assert
            Assert.Equal ( 20, value );
        }

        [Fact]
        public void CompleteEvent_Completed () {
            //arrange
            var externalId = ConveptOfEventExternal.Create ( new ConceptOfEvent { IntValue = 30 } );

            //act
            ConveptOfEventExternal.CompleteEvent ( externalId );

            //assert
            var result = ConveptOfEventExternal.m_events.TryGetValue ( externalId, out var _ );
            Assert.False ( result );
        }

    }


}
