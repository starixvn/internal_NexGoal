import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';

import { getRoleClassName } from '../../common-functions';
import { PerformanceRecordStatus, PerformanceRecordType } from '../../constants';
import getResource from '../../resourceHelper';

export default class PerformanceRecord extends Component {
    static propTypes = {
        objectId: PropTypes.number,
        employees: PropTypes.arrayOf(PropTypes.object),
        creator: PropTypes.object,
        submittedDate: PropTypes.string,
        modifiedDate: PropTypes.string,
        isHost: PropTypes.bool,
        comment: PropTypes.string,
        grade: PropTypes.object,
        projects: PropTypes.arrayOf(PropTypes.string),
        roleId: PropTypes.number,
        statusId: PropTypes.number,
        isHighlighted: PropTypes.bool,
        onEdit: PropTypes.func,
        type: PropTypes.number,
        nomination: PropTypes.object
    };

    static defaultProps = {
        objectId: 0,
        employees: [],
        creator: { employeeId: 0, nickName: '', fullName: '', avatar: '' },
        submittedDate: '',
        modifiedDate: '',
        isHost: false,
        comment: '',
        grade: null,
        projects: [''],
        roleId: 0,
        statusId: 0,
        isHighlighted: false,
        onEdit: function () { },
        type: null,
        nomination: null
    };

    constructor(props) {
        super(props);

        this.state = {
            showCollapseExpandButton: false,
            isExpanded: false
        };
    }

    componentDidMount() {
        this.detectCollapseExpandComment();
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (this.props.objectId !== nextProps.objectId
            || JSON.stringify(this.props.employees) !== JSON.stringify(nextProps.employees)
            || JSON.stringify(this.props.creator) !== JSON.stringify(nextProps.creator)
            || this.props.submittedDate !== nextProps.submittedDate
            || this.props.modifiedDate !== nextProps.modifiedDate
            || this.props.isHost !== nextProps.isHost
            || this.props.comment !== nextProps.comment
            || JSON.stringify(this.props.grade) !== JSON.stringify(nextProps.grade)
            || JSON.stringify(this.state.projects) !== JSON.stringify(nextState.projects)
            || this.props.roleId !== nextProps.roleId
            || this.props.statusId !== nextProps.statusId
            || this.props.isHighlighted !== nextProps.isHighlighted
            || this.state.showCollapseExpandButton !== nextState.showCollapseExpandButton
            || this.state.isExpanded !== nextState.isExpanded) {
            return true;
        }

        return false;
    }

    collapseExpandComment = () => {
        this.setState({
            isExpanded: !this.state.isExpanded
        });
    }

    detectCollapseExpandComment() {
        let height = this.element.offsetHeight;
        if (height > 60) {
            this.setState({
                showCollapseExpandButton: true,
                isExpanded: true
            });
        }
    }

    handleEdit = () => {
        this.props.onEdit(this.props.objectId);
    }

    render() {
        let employees = this.props.employees;
        let creator = this.props.creator;
        let projects = this.props.projects && this.props.projects.length > 0 ? this.props.projects.map((e, i) => <span key={i} className="tag">{e}</span>) : 'N/A';

        return (
            <div className={`record ${this.props.isHighlighted ? 'highlighted' : ''}`} id={`record-${this.props.objectId}`}>
                <div className="avatar-user">
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(this.props.roleId)}>{creator.nickName}</span>
                        {
                            employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
                            <Fragment>
                                <span className="icon-checkin" />
                                <span className="role-member">{employees.map(employee => { return employee.nickName; }).join(', ')}</span>
                            </Fragment>
                        }
                        <div>
                            <span className="submit-date">{this.props.submittedDate}</span>
                            {
                                this.props.statusId === PerformanceRecordStatus.Draft &&
                                <span className="draft">draft</span>
                            }
                        </div>

                    </div>
                    {
                        (this.props.modifiedDate || this.props.isHost) &&
                        <div className="date">
                            <div className="edit-date">
                                {this.props.type === PerformanceRecordType.Nomination && <Fragment>{this.props.nomination.period.title}</Fragment>}

                                {
                                    this.props.statusId !== PerformanceRecordStatus.Draft && this.props.modifiedDate &&
                                    <Fragment>Edited on {this.props.modifiedDate}</Fragment>
                                }
                                {
                                    this.props.isHost &&
                                    <a className="icon-edit" onClick={this.handleEdit} />
                                }
                            </div>
                        </div>
                    }
                </div>

                <div className="comment" >
                    <p className={this.state.isExpanded ? "collapsed" : "expanded"} ref={(element) => this.element = element}>{this.props.comment}</p>
                    {
                        this.state.showCollapseExpandButton ?
                            <span className={this.state.isExpanded ? "see-more" : "see-less"} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>
                {this.props.type !== PerformanceRecordType.Nomination &&
                    <Fragment>
                        <div className="grade">
                    {`${getResource('Grade')}:`} <div className="grade-detail">{this.props.grade ? this.props.grade.name : ''}</div>
                        </div>
                        <div className="project">
                    {`${getResource('ProjectTag')}:`} <div className="project-detail">{projects}</div>
                        </div>
                    </Fragment>
                }
            </div >
        );
    }
}