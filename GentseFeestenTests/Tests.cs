using Domain.Exceptions;
using Domain.Models;

namespace GentseFeestenTests
{
    public class Tests
    {
        // Test #1 | 1. Een dagplan kan slechts aangemaakt worden op een dag waarop evenementen plaatsvinden.
        [Fact]
        public void DagplanShouldBeCreatedOnlyIfThereAreEventsOnThatDate()
        {
            // Arrange
            Gebruiker gebruiker = new Gebruiker("Almendrit", "Sadriu", 50m);
            Evenement evenement1 = new Evenement("123", "Test Event 1", new DateTime(2020, 8, 18, 10, 0, 0), new DateTime(2020, 8, 18, 12, 0, 0), 30m, "testen moet ook");

            Dagplan dagplan = new Dagplan(new DateTime(2023, 8, 18), gebruiker);
            dagplan.VoegEvenementToe(evenement1);

            // Act & Assert
            Assert.Throws<GentseFeestenException>(() => gebruiker.VoegDagplanToe(dagplan));
        }

        // Test #2 | Hetzelfde evenement kan slechts één maal gepland worden gedurende de Gentse feesten.
        [Fact]
        public void SameEventCannotBeScheduledTwiceForSameUser()
        {
            // Arrange
            Gebruiker gebruiker = new Gebruiker("Almendrit", "Sadriu", 50m);
            DateTime date = new DateTime(2023, 8, 18);

            // Create an event
            Evenement evenement = new Evenement("123", "Test Event", new DateTime(2023, 8, 18, 20, 30, 0), new DateTime(2023, 8, 18, 21, 30, 0), 1m, "testen moet ook");

            // Act
            Dagplan dagplan = new Dagplan(date, gebruiker);

            // Add the event to the dagplan
            dagplan.VoegEvenementToe(evenement);

            // Assert
            // Attempting to add the same event again should throw an exception
            Assert.Throws<GentseFeestenException>(() => dagplan.VoegEvenementToe(evenement));
        }

        // Test #3 | 3. Een evenement in een dagplan mag niet overlappen met een ander evenement.
        [Fact]
        public void EventsShouldOverlap()
        {
            // Arrange
            Evenement evenement1 = new Evenement("123", "test", new DateTime(2023, 8, 18, 20, 30, 0), new DateTime(2023, 8, 18, 21, 30, 0), 1m, "testen moet ook");
            Evenement evenement2 = new Evenement("123", "test", new DateTime(2023, 8, 18, 20, 30, 0), new DateTime(2023, 8, 18, 21, 30, 0), 1m, "testen moet ook");

            // Act
            bool overlap = evenement1.EvenementOverlapt(evenement2);

            // Assert
            Assert.True(overlap);
        }

        // Test #4 | 4. Een evenement moet natuurlijk plaatsvinden op dezelfde datum als de datum waarvoor het dagplan gemaakt wordt.
        [Fact]
        public void EventShouldOccurOnSameDateAsDagplan()
        {
            // Arrange
            Gebruiker gebruiker = new Gebruiker("Almendrit", "Sadriu", 50m);
            DateTime date = new DateTime(2023, 8, 1);
            Dagplan dagplan = new Dagplan(date, gebruiker);

            // Create an event with a different date than the dagplan
            Evenement evenement = new Evenement("123", "Test Event", new DateTime(2023, 8, 18, 20, 30, 0), new DateTime(2023, 8, 18, 21, 30, 0), 1m, "testen moet ook");

            // Act & Assert
            // Assert that adding an event with a different date than the dagplan throws an exception
            Assert.Throws<GentseFeestenException>(() => dagplan.VoegEvenementToe(evenement));
        }


        // Test #5 | 5. Een gebruiker mag voor een bepaalde dag slechts één dagplan aanmaken.
        [Fact]
        public void UserShouldCreateOnlyOneDagplanPerDay()
        {
            // Arrange
            Gebruiker gebruiker = new Gebruiker("Almendrit", "Sadriu", 50m);
            DateTime date = new DateTime(2023, 8, 18);

            // Act
            Dagplan dagplan1 = new Dagplan(date, gebruiker);
            Dagplan dagplan2 = new Dagplan(date, gebruiker);

            // Assert
            gebruiker.VoegDagplanToe(dagplan1); // Add the first dagplan
                                                // Attempting to add a second dagplan for the same day should throw an exception
            Assert.Throws<GentseFeestenException>(() => gebruiker.VoegDagplanToe(dagplan2));
        }

        // Test #6 | 6. De kostprijs van alle evenementen samen per dag mag het beschikbare dagbedrag van degebruiker niet overschrijden.
        [Fact]
        public void TotalCostShouldNotExceedUserBudget()
        {
            // Arrange
            Gebruiker gebruiker = new Gebruiker("Almendrit", "Sadriu", 50m);
            Evenement evenement1 = new Evenement("123", "Test Event 1", new DateTime(2023, 8, 18, 10, 0, 0), new DateTime(2023, 8, 18, 12, 0, 0), 30m, "testen moet ook");
            Evenement evenement2 = new Evenement("456", "Test Event 2", new DateTime(2023, 8, 18, 14, 0, 0), new DateTime(2023, 8, 18, 16, 0, 0), 30m, "testen moet ook");

            Dagplan dagplan = new Dagplan(new DateTime(2023, 8, 18), gebruiker);

            // Act
            // Adding events to the dagplan
            dagplan.VoegEvenementToe(evenement1);

            // Assert
            // The total cost of events in the dagplan should not exceed the user's budget
            Assert.Throws<GentseFeestenException>(() => dagplan.VoegEvenementToe(evenement2));
        }
    }
}