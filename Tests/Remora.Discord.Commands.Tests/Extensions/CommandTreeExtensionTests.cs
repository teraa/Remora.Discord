//
//  CommandTreeExtensionTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using System.Linq;
using Remora.Commands.Trees;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Tests.Data.DiscordLimits;
using Remora.Discord.Commands.Tests.Data.Exclusion;
using Remora.Discord.Commands.Tests.Data.InternalLimits;
using Remora.Discord.Commands.Tests.Data.Valid;
using Remora.Discord.Commands.Tests.Data.Valid.Basics;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Xunit;
using static Remora.Discord.API.Abstractions.Objects.ApplicationCommandOptionType;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
namespace Remora.Discord.Commands.Tests.Extensions;

/// <summary>
/// Tests the <see cref="CommandTreeExtensions"/> class.
/// </summary>
public class CommandTreeExtensionTests
{
    /// <summary>
    /// Tests the <see cref="CommandTreeExtensions.CreateApplicationCommands"/> method.
    /// </summary>
    public class CreateApplicationCommands
    {
        /// <summary>
        /// Tests various failing cases.
        /// </summary>
        public class Failures
        {
            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfGroupsAreTooDeeplyNested()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<TooDeeplyNested>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfACommandHasTooManyParameters()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<TooManyCommandParameters>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfThereAreTooManyRootLevelCommands()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<TooManyCommands>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfAGroupHasTooManyCommands()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<TooManyGroupCommands>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfACommandContainsACollectionParameter()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<CollectionsAreNotSupported>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfACommandContainsASwitchParameter()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<SwitchesAreNotSupported>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfThereAreOverloadsAtTheRootLevel()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<OverloadsAreNotSupportedInRoot>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfThereAreOverloadsInAGroup()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<OverloadsAreNotSupportedInGroups>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfACommandIsTooLong()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<TooLongCommand>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfACommandDescriptionIsTooLong()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<TooLongCommandDescription>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfAParameterDescriptionIsTooLong()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<TooLongParameterDescription>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfMultipleNamedGroupsWithTheSameNameHaveADefaultPermissionAttribute()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<AtMostOneDefaultPermissionAttributeAllowed.Named.GroupOne>();
                builder.RegisterModule<AtMostOneDefaultPermissionAttributeAllowed.Named.GroupTwo>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfContextMenuHasDescription()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ContextMenusWithDescriptionsAreNotSupported>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfContextMenuIsNested()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<NestedContextMenusAreNotSupported>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfContextMenuHasParameters()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ContextMenusWithParametersAreNotSupported>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfChannelTypesAttributeAppliedOnNonChannelParameter()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ChannelTypesAttributeOnlyOnChannelParameter>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }

            /// <summary>
            /// Tests whether method responds appropriately to a failure case.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulIfChannelTypesAttributeHasZeroValues()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ChannelTypesAttributeRequiresAtLeastOneValue>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Unsuccessful(result);
            }
        }

        /// <summary>
        /// Tests various successful cases.
        /// </summary>
        public class Successes
        {
            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void ReturnsSuccessForValidTree()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ValidCommandGroup>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesValidTreeCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ValidCommandGroup>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);

                Assert.NotNull(commands);
                Assert.Equal(2, commands.Count);

                var topLevelCommand = commands.FirstOrDefault(c => c.Name == "top-level-command");
                Assert.NotNull(topLevelCommand);

                var topLevelGroup = commands.FirstOrDefault(c => c.Name == "top-level-group");
                Assert.NotNull(topLevelGroup);

                Assert.True(topLevelGroup!.Options.HasValue);
                Assert.Equal(2, topLevelGroup.Options.Value.Count);

                var firstNestedCommand = topLevelGroup.Options.Value.FirstOrDefault(c => c.Type == SubCommand);
                Assert.NotNull(firstNestedCommand);

                var nestedGroup = topLevelGroup.Options.Value.FirstOrDefault(c => c.Type == SubCommandGroup);
                Assert.NotNull(nestedGroup);

                Assert.True(nestedGroup!.Options.HasValue);
                Assert.Single(nestedGroup.Options.Value);

                var secondNestedCommand = nestedGroup.Options.Value.FirstOrDefault(c => c.Type == SubCommand);
                Assert.NotNull(secondNestedCommand);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesTypedOptionsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<TypedCommands>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                void AssertExistsWithType(string commandName, ApplicationCommandOptionType type)
                {
                    var command = commands.FirstOrDefault(c => c.Name == commandName);
                    Assert.NotNull(command);

                    var parameter = command!.Options.Value[0];
                    Assert.Equal(type, parameter.Type);
                }

                AssertExistsWithType("sbyte-value", Integer);
                AssertExistsWithType("byte-value", Integer);
                AssertExistsWithType("short-value", Integer);
                AssertExistsWithType("ushort-value", Integer);
                AssertExistsWithType("int-value", Integer);
                AssertExistsWithType("uint-value", Integer);
                AssertExistsWithType("long-value", Integer);
                AssertExistsWithType("ulong-value", Integer);

                AssertExistsWithType("float-value", Number);
                AssertExistsWithType("double-value", Number);
                AssertExistsWithType("decimal-value", Number);

                AssertExistsWithType("string-value", String);

                AssertExistsWithType("bool-value", Boolean);

                AssertExistsWithType("role-value", ApplicationCommandOptionType.Role);
                AssertExistsWithType("user-value", ApplicationCommandOptionType.User);

                AssertExistsWithType("channel-value", ApplicationCommandOptionType.Channel);
                var channelCommand = commands.First(c => c.Name == "channel-value");
                var channelParameter = channelCommand.Options.Value[0];
                Assert.False(channelParameter.ChannelTypes.HasValue);

                AssertExistsWithType("typed-channel-value", ApplicationCommandOptionType.Channel);
                var typedChannelCommand = commands.First(c => c.Name == "typed-channel-value");
                var typedChannelParameter = typedChannelCommand.Options.Value[0];
                Assert.True(typedChannelParameter.ChannelTypes.HasValue);
                Assert.True(typedChannelParameter.ChannelTypes.Value.Count == 1);
                Assert.True(typedChannelParameter.ChannelTypes.Value[0] == ChannelType.GuildText);

                AssertExistsWithType("member-value", ApplicationCommandOptionType.User);

                AssertExistsWithType("enum-value", String);
                var enumCommand = commands.First(c => c.Name == "enum-value");
                var enumParameter = enumCommand.Options.Value[0];
                Assert.True(enumParameter.Choices.HasValue);

                var enumChoices = enumParameter.Choices.Value;
                Assert.Equal(2, enumChoices.Count);
                Assert.Collection
                (
                    enumChoices,
                    choice =>
                    {
                        Assert.Equal(nameof(TestEnum.Value1), choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(TestEnum.Value1), choice.Value.AsT0);
                    },
                    choice =>
                    {
                        Assert.Equal(nameof(TestEnum.Value2), choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(TestEnum.Value2), choice.Value.AsT0);
                    }
                );

                AssertExistsWithType("hint-value", ApplicationCommandOptionType.Role);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesDescriptionOverriddenEnumOptionsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupWithEnumParameterWithDescriptionOverrides>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                void AssertExistsWithType(string commandName, ApplicationCommandOptionType type)
                {
                    var command = commands.FirstOrDefault(c => c.Name == commandName);
                    Assert.NotNull(command);

                    var parameter = command!.Options.Value[0];
                    Assert.Equal(type, parameter.Type);
                }

                AssertExistsWithType("description-enum", String);
                var enumCommand = commands.First(c => c.Name == "description-enum");

                var enumParameter = enumCommand.Options.Value[0];
                Assert.True(enumParameter.Choices.HasValue);

                var enumChoices = enumParameter.Choices.Value;
                Assert.Equal(2, enumChoices.Count);
                Assert.Collection
                (
                    enumChoices,
                    choice =>
                    {
                        Assert.Equal("A longer description", choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DescriptionEnum.A), choice.Value.AsT0);
                    },
                    choice =>
                    {
                        Assert.Equal(nameof(DescriptionEnum.B), choice.Name);
                        Assert.True(choice.Value.IsT0);
                        Assert.Equal(nameof(DescriptionEnum.B), choice.Value.AsT0);
                    }
                );
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesRequiredOptionsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<CommandsWithRequiredOrOptionalParameters>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                var requiredCommand = commands.First(c => c.Name == "required");
                var requiredParameter = requiredCommand.Options.Value[0];
                Assert.True(requiredParameter.IsRequired.HasValue);
                Assert.True(requiredParameter.IsRequired.Value);

                var optionalCommand = commands.First(c => c.Name == "optional");
                var optionalParameter = optionalCommand.Options.Value[0];
                if (optionalParameter.IsRequired.HasValue)
                {
                    Assert.False(optionalParameter.IsRequired.Value);
                }
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesUnnamedGroupWithDefaultPermissionCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<UnnamedGroupWithDefaultPermission>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                var command = commands.SingleOrDefault();
                Assert.True(command!.DefaultPermission.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesNamedGroupWithDefaultPermissionCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<NamedGroupWithDefaultPermission>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                ResultAssert.Successful(result);
                Assert.NotNull(commands);

                var command = commands.SingleOrDefault();
                Assert.True(command!.DefaultPermission.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesUngroupedTopLevelCommandsWithDefaultPermissionCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<MultipleCommandsWithDefaultPermission.GroupOne>();
                builder.RegisterModule<MultipleCommandsWithDefaultPermission.GroupTwo>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                Assert.Equal(2, commands.Count);
                var a = commands[0];
                var b = commands[1];

                Assert.True(a.DefaultPermission.Value);
                Assert.False(b.DefaultPermission.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesContextMenuCommandsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupWithContextMenus>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                Assert.Equal(2, commands.Count);

                var user = commands[0];
                var message = commands[1];

                Assert.Equal(ApplicationCommandType.User, user.Type.Value);
                Assert.Equal(ApplicationCommandType.Message, message.Type.Value);
            }

            /// <summary>
            /// Tests whether the method responds appropriately to a successful case.
            /// </summary>
            [Fact]
            public void CreatesCombinedContextMenuCommandsCorrectly()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupWithContextMenuAndCommand>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                ResultAssert.Successful(result);

                var commands = result.Entity;

                Assert.Equal(2, commands.Count);

                var normal = commands[0];
                var message = commands[1];

                Assert.Equal(ApplicationCommandType.ChatInput, normal.Type.Value);
                Assert.Equal(ApplicationCommandType.Message, message.Type.Value);
            }
        }

        /// <summary>
        /// Tests various cases where commands are filtered out.
        /// </summary>
        public class Filtering
        {
            /// <summary>
            /// Tests whether a single command can be excluded using
            /// <see cref="ExcludeFromSlashCommandsAttribute"/>.
            /// </summary>
            [Fact]
            public void CanExcludeCommand()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ExcludedCommand>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                var group = commands.Single();

                Assert.Equal("a", group.Name);

                Assert.Collection
                (
                    group.Options.Value,
                    c =>
                    {
                        Assert.Equal(SubCommand, c.Type);
                        Assert.Equal("b", c.Name);
                    }
                );
            }

            /// <summary>
            /// Tests whether a single nested command can be excluded using
            /// <see cref="ExcludeFromSlashCommandsAttribute"/>.
            /// </summary>
            [Fact]
            public void CanExcludeNestedCommand()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ExcludedNestedCommand>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                var group = commands.Single();

                Assert.Equal("a", group.Name);

                var nestedGroup = @group.Options.Value.Single();

                Assert.Equal(SubCommandGroup, nestedGroup.Type);
                Assert.Equal("b", nestedGroup.Name);

                Assert.Collection
                (
                    nestedGroup.Options.Value,
                    c =>
                    {
                        Assert.Equal(SubCommand, c.Type);
                        Assert.Equal("d", c.Name);
                    }
                );
            }

            /// <summary>
            /// Tests whether a complete group can be excluded using
            /// <see cref="ExcludeFromSlashCommandsAttribute"/>.
            /// </summary>
            [Fact]
            public void CanExcludeGroup()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ExcludedGroup>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                Assert.Empty(commands);
            }

            /// <summary>
            /// Tests whether a complete nested group can be excluded using
            /// <see cref="ExcludeFromSlashCommandsAttribute"/>.
            /// </summary>
            [Fact]
            public void CanExcludeNestedGroup()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<ExcludedNestedGroup>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                var group = commands.Single();

                Assert.Equal("a", group.Name);

                Assert.Collection
                (
                    group.Options.Value,
                    c =>
                    {
                        Assert.Equal(SubCommand, c.Type);
                        Assert.Equal("d", c.Name);
                    }
                );
            }

            /// <summary>
            /// Tests whether groups that are empty after exclusion filtering are optimized out.
            /// </summary>
            [Fact]
            public void EmptyGroupsAreOptimizedOut()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<GroupThatIsEmptyAfterExclusion>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                Assert.Empty(commands);
            }

            /// <summary>
            /// Tests whether nested groups that are empty after exclusion filtering are optimized out.
            /// </summary>
            [Fact]
            public void NestedEmptyGroupsAreOptimizedOut()
            {
                var builder = new CommandTreeBuilder();
                builder.RegisterModule<NestedGroupThatIsEmptyAfterExclusion>();

                var tree = builder.Build();

                var result = tree.CreateApplicationCommands();
                var commands = result.Entity;

                Assert.Empty(commands);
            }
        }
    }

    /// <summary>
    /// Tests the <see cref="CommandTreeExtensions.MapDiscordCommands"/> method.
    /// </summary>
    public class MapDiscordCommands
    {
        /// <summary>
        /// Tests whether the method can successfully map a single top-level command.
        /// </summary>
        [Fact]
        public void CanMapSingleTopLevelCommand()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<UnnamedGroupWithSingleCommand>();

            var tree = treeBuilder.Build();

            var commandNode = tree.Root.Children[0];

            var commandID = DiscordSnowflake.New(1);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandID,
                    default,
                    default,
                    default,
                    commandNode.Key,
                    string.Empty,
                    default,
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Single(map);
            var (id, node) = map.Single();

            Assert.Equal((default, commandID), id);
            Assert.Same(commandNode, node.AsT1);
        }

        /// <summary>
        /// Tests whether the method can successfully map multiple top-level commands.
        /// </summary>
        [Fact]
        public void CanMapMultipleTopLevelCommands()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<UnnamedGroupWithMultipleCommands>();

            var tree = treeBuilder.Build();

            var commandNodeA = tree.Root.Children[0];
            var commandNodeB = tree.Root.Children[1];

            var commandAID = DiscordSnowflake.New(1);
            var commandBID = DiscordSnowflake.New(2);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandAID,
                    default,
                    default,
                    default,
                    commandNodeA.Key,
                    string.Empty,
                    default,
                    default,
                    default
                ),
                new ApplicationCommand
                (
                    commandBID,
                    default,
                    default,
                    default,
                    commandNodeB.Key,
                    string.Empty,
                    default,
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Equal(2, map.Count);
            var (nodeAID, nodeA) = map.ToList()[0];
            var (nodeBID, nodeB) = map.ToList()[1];

            Assert.Equal((default, commandAID), nodeAID);
            Assert.Same(commandNodeA, nodeA.AsT1);

            Assert.Equal((default, commandBID), nodeBID);
            Assert.Same(commandNodeB, nodeB.AsT1);
        }

        /// <summary>
        /// Tests whether the method can successfully map a single nested command.
        /// </summary>
        [Fact]
        public void CanMapSingleNestedCommand()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<NamedGroupWithSingleCommand>();

            var tree = treeBuilder.Build();

            var groupNode = (GroupNode)tree.Root.Children[0];
            var commandNode = groupNode.Children[0];

            var commandID = DiscordSnowflake.New(1);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandID,
                    default,
                    default,
                    default,
                    groupNode.Key,
                    string.Empty,
                    new[]
                    {
                        new ApplicationCommandOption(SubCommand, commandNode.Key, string.Empty)
                    },
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Single(map);
            var (id, value) = map.Single();

            Assert.Equal((default, commandID), id);

            var subMap = value.AsT0;
            Assert.Single(subMap);

            var (path, mappedNode) = subMap.Single();
            Assert.Equal("a::b", path);
            Assert.Same(commandNode, mappedNode);
        }

        /// <summary>
        /// Tests whether the method can successfully map a single nested command.
        /// </summary>
        [Fact]
        public void CanMapMultipleNestedCommand()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<NamedGroupWithMultipleCommands>();

            var tree = treeBuilder.Build();

            var groupNode = (GroupNode)tree.Root.Children[0];
            var commandNodeB = groupNode.Children[0];
            var commandNodeC = groupNode.Children[1];

            var commandID = DiscordSnowflake.New(1);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandID,
                    default,
                    default,
                    default,
                    groupNode.Key,
                    string.Empty,
                    new[]
                    {
                        new ApplicationCommandOption(SubCommand, commandNodeB.Key, string.Empty),
                        new ApplicationCommandOption(SubCommand, commandNodeC.Key, string.Empty)
                    },
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Single(map);
            var (id, value) = map.Single();

            Assert.Equal((default, commandID), id);

            var subMap = value.AsT0;
            Assert.Equal(2, subMap.Count);

            var (pathB, mappedNodeB) = subMap.ToList()[0];
            var (pathC, mappedNodeC) = subMap.ToList()[1];

            Assert.Equal("a::b", pathB);
            Assert.Equal("a::c", pathC);

            Assert.Same(commandNodeB, mappedNodeB);
            Assert.Same(commandNodeC, mappedNodeC);
        }

        /// <summary>
        /// Tests whether the method can successfully map a single deeply nested command.
        /// </summary>
        [Fact]
        public void CanMapDeeplyNestedCommand()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<DeeplyNestedCommand>();

            var tree = treeBuilder.Build();

            var groupNode = (GroupNode)tree.Root.Children[0];
            var subGroupNode = (GroupNode)groupNode.Children[0];
            var commandNode = subGroupNode.Children[0];

            var commandID = DiscordSnowflake.New(1);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    commandID,
                    default,
                    default,
                    default,
                    groupNode.Key,
                    string.Empty,
                    new[]
                    {
                        new ApplicationCommandOption(SubCommandGroup, subGroupNode.Key, string.Empty, Options: new[]
                        {
                            new ApplicationCommandOption(SubCommand, commandNode.Key, string.Empty)
                        })
                    },
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            Assert.Single(map);
            var (id, value) = map.Single();

            Assert.Equal((default, commandID), id);

            var subMap = value.AsT0;
            Assert.Single(subMap);

            var (path, mappedNode) = subMap.Single();
            Assert.Equal("a::b::c", path);
            Assert.Same(commandNode, mappedNode);
        }

        /// <summary>
        /// Tests whether the method can successfully map a complex tree, which is a combination of all previous
        /// tests.
        /// </summary>
        [Fact]
        public void CanMapComplexTree()
        {
            var treeBuilder = new CommandTreeBuilder();
            treeBuilder.RegisterModule<ComplexGroup>();
            treeBuilder.RegisterModule<ComplexGroupUnnamedPart>();

            var tree = treeBuilder.Build();

            var groupNode = (GroupNode)tree.Root.Children.Single(c => c.Key is "a");
            var subGroupNode = (GroupNode)groupNode.Children.Single(c => c.Key is "b");
            var commandNodeC = subGroupNode.Children.Single(c => c.Key is "c");
            var commandNodeD = subGroupNode.Children.Single(c => c.Key is "d");
            var commandNodeE = groupNode.Children.Single(c => c.Key is "e");
            var commandNodeF = groupNode.Children.Single(c => c.Key is "f");
            var commandNodeG = tree.Root.Children.Single(c => c.Key is "g");

            var groupID = DiscordSnowflake.New(1);
            var commandGID = DiscordSnowflake.New(2);
            var applicationCommands = new List<IApplicationCommand>
            {
                new ApplicationCommand
                (
                    groupID,
                    default,
                    default,
                    default,
                    groupNode.Key,
                    string.Empty,
                    new[]
                    {
                        new ApplicationCommandOption(SubCommandGroup, subGroupNode.Key, string.Empty, Options: new[]
                        {
                            new ApplicationCommandOption(SubCommand, commandNodeC.Key, string.Empty),
                            new ApplicationCommandOption(SubCommand, commandNodeD.Key, string.Empty),
                        }),
                        new ApplicationCommandOption(SubCommand, commandNodeE.Key, string.Empty),
                        new ApplicationCommandOption(SubCommand, commandNodeF.Key, string.Empty),
                    },
                    default,
                    default
                ),
                new ApplicationCommand
                (
                    commandGID,
                    default,
                    default,
                    default,
                    commandNodeG.Key,
                    string.Empty,
                    default,
                    default,
                    default
                )
            };

            var map = tree.MapDiscordCommands(applicationCommands);

            var (mappedGroupID, groupMap) = map.ToList()[0];

            Assert.Equal(mappedGroupID.CommandID, groupID);
            var (pathC, mappedCommandNodeC) = groupMap.AsT0.ToList()[0];
            var (pathD, mappedCommandNodeD) = groupMap.AsT0.ToList()[1];
            var (pathE, mappedCommandNodeE) = groupMap.AsT0.ToList()[2];
            var (pathF, mappedCommandNodeF) = groupMap.AsT0.ToList()[3];

            Assert.Equal("a::b::c", pathC);
            Assert.Equal("a::b::d", pathD);
            Assert.Equal("a::e", pathE);
            Assert.Equal("a::f", pathF);

            Assert.Same(mappedCommandNodeC, commandNodeC);
            Assert.Same(mappedCommandNodeD, commandNodeD);
            Assert.Same(mappedCommandNodeE, commandNodeE);
            Assert.Same(mappedCommandNodeF, commandNodeF);

            var (mappedCommandID, mappedCommandNode) = map.ToList()[1];

            Assert.Equal(mappedCommandID.CommandID, commandGID);
            Assert.Same(commandNodeG, mappedCommandNode.AsT1);
        }
    }
}
