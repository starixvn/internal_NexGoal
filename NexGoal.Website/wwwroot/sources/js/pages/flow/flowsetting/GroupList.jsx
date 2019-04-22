import React, { Component } from 'react';

export default class GroupList extends Component {
    renderGroups = group =>
        (<div key={group.groupId} className={group.hasSetting ? 'has-setting' : ''} onClick={this.props.onClick.bind(this, group)}>
            {group.name}
        </div>)

    render() {
        return (
            <div className="evaluator-list">
                <div className="header">Team List</div>
                <div className="content">
                    {this.props.groups.map(this.renderGroups)}
                </div>
            </div>
        );
    }
}