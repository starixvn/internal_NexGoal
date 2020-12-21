import React, { Component } from 'react';

export default class GroupList extends Component {
    renderGroup = group =>
        (<div key={group.groupId} className={group.hasSetting ? 'has-setting' : ''} onClick={this.props.onClick.bind(this, group)}>
            {group.name}
        </div>);

    render() {
        return (
            <div className="group-list">
                <div className="header">Team List</div>
                <div className="content">
                    {this.props.groups.map(this.renderGroup)}
                </div>
            </div>
        );
    }
}